using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Acti2SHM
{
    public partial class Frm_MainForm : Form
    {
        double q0, q1, q2, q3;
        double beta;
        double sampleFreq;
        UInt16 AXCOL, AYCOL, AZCOL, GXCOL, GYCOL, GZCOL, MXCOL, MYCOL, MZCOL; /* Column Numbers for the CSV */
        int actigraphHeaderLines; /* Number of lines in the Actigraph Header to Ignore */
        char separator;
        String inputFileName;
        String outputFileName;
        int BytesPerLine;   /* Precomputed value of average bytes per line in Actigraph CSV. Used to estimate progress */
        ManualResetEvent runThread = new ManualResetEvent(false);
        Thread t;
        bool EndThread;
        BinaryWriter DataWriter;
        StreamReader reader;
        StreamWriter EventsWriter;
        String ExtractDateString;
        bool datefound = false;

        enum Devices { Invensense, Actigraph, iPhone };
        Devices CurrentDevice;

        public Frm_MainForm()
        {
            InitializeComponent();

            /* Init values */
            separator = ',';
            AXCOL = 1; AYCOL = 2; AZCOL = 3; GXCOL = 5; GYCOL = 6; GZCOL = 7; MXCOL = 8; MYCOL = 9; MZCOL = 10;
            sampleFreq = 15.0f;
            beta = 0.041;
            BytesPerLine = (int) ((long)1018925758 / (long)8200000);
            actigraphHeaderLines = 11;
        }

        /// <summary>
        /// Calculate Gravity (GravX, GravY, GravZ) from quaternions
        /// </summary>
        /// <param name="_q0"></param>
        /// <param name="_q1"></param>
        /// <param name="_q2"></param>
        /// <param name="_q3"></param>
        /// <param name="GravX"></param>
        /// <param name="GravY"></param>
        /// <param name="GravZ"></param>
        void GetGravity(double _q0, double _q1, double _q2, double _q3, out double GravX, out double GravY, out double GravZ)
        {
            double[,] R = new double[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            double sq__q1 = 2 * _q1 * _q1;
            double sq__q2 = 2 * _q2 * _q2;
            double sq__q3 = 2 * _q3 * _q3;
            double _q1__q2 = 2 * _q1 * _q2;
            double _q3__q0 = 2 * _q3 * _q0;
            double _q1__q3 = 2 * _q1 * _q3;
            double _q2__q0 = 2 * _q2 * _q0;
            double _q2__q3 = 2 * _q2 * _q3;
            double _q1__q0 = 2 * _q1 * _q0;

            R[0, 0] = 1 - sq__q2 - sq__q3;
            R[0, 1] = _q1__q2 - _q3__q0;
            R[0, 2] = _q1__q3 + _q2__q0;
            R[1, 0] = _q1__q2 + _q3__q0;
            R[1, 1] = 1 - sq__q1 - sq__q3;
            R[1, 2] = _q2__q3 - _q1__q0;
            R[2, 0] = _q1__q3 - _q2__q0;
            R[2, 1] = _q2__q3 + _q1__q0;
            R[2, 2] = 1 - sq__q1 - sq__q2;

            /* Seperating gravity */
            GravX = R[2, 0];
            GravY = R[2, 1];
            GravZ = R[2, 2];
        }

        void MadgwickAHRSupdate(double gx, double gy, double gz, double ax, double ay, double az, double mx, double my, double mz)
        {
            double recipNorm;
            double s0, s1, s2, s3;
            double qDot1, qDot2, qDot3, qDot4;
            double hx, hy;// _8bx, _8bz;
            double _2q0mx, _2q0my, _2q0mz, _2q1mx, _2bx, _2bz, _4bx, _4bz, _2q0, _2q1, _2q2, _2q3, _2q0q2, _2q2q3, q0q0, q0q1, q0q2, q0q3, q1q1, q1q2, q1q3, q2q2, q2q3, q3q3;

            /* Convert gyro data from deg/sec to rad/sec */
            gx = gx * Math.PI / 180;
            gy = gy * Math.PI / 180;
            gz = gz * Math.PI / 180;

            // Use IMU algorithm if magnetometer measurement invalid (avoids NaN in magnetometer normalisation)
            if ((mx == 0.0f) && (my == 0.0f) && (mz == 0.0f))
            {
                MadgwickAHRSupdateIMU(gx, gy, gz, ax, ay, az);
                return;
            }

            // Rate of change of quaternion from gyroscope
            qDot1 = 0.5f * (-q1 * gx - q2 * gy - q3 * gz);
            qDot2 = 0.5f * (q0 * gx + q2 * gz - q3 * gy);
            qDot3 = 0.5f * (q0 * gy - q1 * gz + q3 * gx);
            qDot4 = 0.5f * (q0 * gz + q1 * gy - q2 * gx);

            // Compute feedback only if accelerometer measurement valid (avoids NaN in accelerometer normalisation)
            if (!((ax == 0.0f) && (ay == 0.0f) && (az == 0.0f)))
            {

                // Normalise accelerometer measurement
                recipNorm = 1 / Math.Sqrt(ax * ax + ay * ay + az * az);
                ax *= recipNorm;
                ay *= recipNorm;
                az *= recipNorm;

                // Normalise magnetometer measurement
                recipNorm = 1 / Math.Sqrt(mx * mx + my * my + mz * mz);
                mx *= recipNorm;
                my *= recipNorm;
                mz *= recipNorm;

                // Auxiliary variables to avoid repeated arithmetic
                _2q0mx = 2.0f * q0 * mx;
                _2q0my = 2.0f * q0 * my;
                _2q0mz = 2.0f * q0 * mz;
                _2q1mx = 2.0f * q1 * mx;
                _2q0 = 2.0f * q0;
                _2q1 = 2.0f * q1;
                _2q2 = 2.0f * q2;
                _2q3 = 2.0f * q3;
                _2q0q2 = 2.0f * q0 * q2;
                _2q2q3 = 2.0f * q2 * q3;
                q0q0 = q0 * q0;
                q0q1 = q0 * q1;
                q0q2 = q0 * q2;
                q0q3 = q0 * q3;
                q1q1 = q1 * q1;
                q1q2 = q1 * q2;
                q1q3 = q1 * q3;
                q2q2 = q2 * q2;
                q2q3 = q2 * q3;
                q3q3 = q3 * q3;

                // Reference direction of Earth's magnetic field
                hx = mx * q0q0 - _2q0my * q3 + _2q0mz * q2 + mx * q1q1 + _2q1 * my * q2 + _2q1 * mz * q3 - mx * q2q2 - mx * q3q3;
                hy = _2q0mx * q3 + my * q0q0 - _2q0mz * q1 + _2q1mx * q2 - my * q1q1 + my * q2q2 + _2q2 * mz * q3 - my * q3q3;
                _2bx = Math.Sqrt(hx * hx + hy * hy);
                _2bz = -_2q0mx * q2 + _2q0my * q1 + mz * q0q0 + _2q1mx * q3 - mz * q1q1 + _2q2 * my * q3 - mz * q2q2 + mz * q3q3;
                _4bx = 2.0f * _2bx;
                _4bz = 2.0f * _2bz;

                // Gradient decent algorithm corrective step
                s0 = -_2q2 * (2.0f * q1q3 - _2q0q2 - ax) + _2q1 * (2.0f * q0q1 + _2q2q3 - ay) - _2bz * q2 * (_2bx * (0.5f - q2q2 - q3q3) + _2bz * (q1q3 - q0q2) - mx) + (-_2bx * q3 + _2bz * q1) * (_2bx * (q1q2 - q0q3) + _2bz * (q0q1 + q2q3) - my) + _2bx * q2 * (_2bx * (q0q2 + q1q3) + _2bz * (0.5f - q1q1 - q2q2) - mz);
                s1 = _2q3 * (2.0f * q1q3 - _2q0q2 - ax) + _2q0 * (2.0f * q0q1 + _2q2q3 - ay) - 4.0f * q1 * (1 - 2.0f * q1q1 - 2.0f * q2q2 - az) + _2bz * q3 * (_2bx * (0.5f - q2q2 - q3q3) + _2bz * (q1q3 - q0q2) - mx) + (_2bx * q2 + _2bz * q0) * (_2bx * (q1q2 - q0q3) + _2bz * (q0q1 + q2q3) - my) + (_2bx * q3 - _4bz * q1) * (_2bx * (q0q2 + q1q3) + _2bz * (0.5f - q1q1 - q2q2) - mz);
                s2 = -_2q0 * (2.0f * q1q3 - _2q0q2 - ax) + _2q3 * (2.0f * q0q1 + _2q2q3 - ay) - 4.0f * q2 * (1 - 2.0f * q1q1 - 2.0f * q2q2 - az) + (-_4bx * q2 - _2bz * q0) * (_2bx * (0.5f - q2q2 - q3q3) + _2bz * (q1q3 - q0q2) - mx) + (_2bx * q1 + _2bz * q3) * (_2bx * (q1q2 - q0q3) + _2bz * (q0q1 + q2q3) - my) + (_2bx * q0 - _4bz * q2) * (_2bx * (q0q2 + q1q3) + _2bz * (0.5f - q1q1 - q2q2) - mz);
                s3 = _2q1 * (2.0f * q1q3 - _2q0q2 - ax) + _2q2 * (2.0f * q0q1 + _2q2q3 - ay) + (-_4bx * q3 + _2bz * q1) * (_2bx * (0.5f - q2q2 - q3q3) + _2bz * (q1q3 - q0q2) - mx) + (-_2bx * q0 + _2bz * q2) * (_2bx * (q1q2 - q0q3) + _2bz * (q0q1 + q2q3) - my) + _2bx * q1 * (_2bx * (q0q2 + q1q3) + _2bz * (0.5f - q1q1 - q2q2) - mz);

                recipNorm = (double)1.00 / (double)Math.Sqrt(s0 * s0 + s1 * s1 + s2 * s2 + s3 * s3); // normalise step magnitude
                s0 *= recipNorm;
                s1 *= recipNorm;
                s2 *= recipNorm;
                s3 *= recipNorm;

                // Apply feedback step
                qDot1 -= beta * s0;
                qDot2 -= beta * s1;
                qDot3 -= beta * s2;
                qDot4 -= beta * s3;

            }

            // Integrate rate of change of quaternion to yield quaternion
            q0 += qDot1 * (1.0f / sampleFreq);
            q1 += qDot2 * (1.0f / sampleFreq);
            q2 += qDot3 * (1.0f / sampleFreq);
            q3 += qDot4 * (1.0f / sampleFreq);

            // Normalise quaternion
            recipNorm = 1 / Math.Sqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
            q0 *= recipNorm;
            q1 *= recipNorm;
            q2 *= recipNorm;
            q3 *= recipNorm;
        }
  
        /// <summary>
        /// Using only 6 DOF to calculate quaternion (when Acceleromter normalization results in NaN).
        /// Uses globals: q0, q1, q2, q3
        /// </summary>
        /// <param name="gx"></param>
        /// <param name="gy"></param>
        /// <param name="gz"></param>
        /// <param name="ax"></param>
        /// <param name="ay"></param>
        /// <param name="az"></param>
        void MadgwickAHRSupdateIMU(double gx, double gy, double gz, double ax, double ay, double az)
        {
            double recipNorm;
            double s0, s1, s2, s3;
            double qDot1, qDot2, qDot3, qDot4;
            double _2q0, _2q1, _2q2, _2q3, _4q0, _4q1, _4q2, _8q1, _8q2, q0q0, q1q1, q2q2, q3q3;

            // Rate of change of quaternion from gyroscope
            qDot1 = 0.5f * (-q1 * gx - q2 * gy - q3 * gz);
            qDot2 = 0.5f * (q0 * gx + q2 * gz - q3 * gy);
            qDot3 = 0.5f * (q0 * gy - q1 * gz + q3 * gx);
            qDot4 = 0.5f * (q0 * gz + q1 * gy - q2 * gx);

            // Compute feedback only if accelerometer measurement valid (avoids NaN in accelerometer normalisation)
            if (!((ax == 0.0f) && (ay == 0.0f) && (az == 0.0f)))
            {

                // Normalise accelerometer measurement
                recipNorm = 1 / Math.Sqrt(ax * ax + ay * ay + az * az);
                ax *= recipNorm;
                ay *= recipNorm;
                az *= recipNorm;

                // Auxiliary variables to avoid repeated arithmetic
                _2q0 = 2.0f * q0;
                _2q1 = 2.0f * q1;
                _2q2 = 2.0f * q2;
                _2q3 = 2.0f * q3;
                _4q0 = 4.0f * q0;
                _4q1 = 4.0f * q1;
                _4q2 = 4.0f * q2;
                _8q1 = 8.0f * q1;
                _8q2 = 8.0f * q2;
                q0q0 = q0 * q0;
                q1q1 = q1 * q1;
                q2q2 = q2 * q2;
                q3q3 = q3 * q3;

                // Gradient decent algorithm corrective step
                s0 = _4q0 * q2q2 + _2q2 * ax + _4q0 * q1q1 - _2q1 * ay;
                s1 = _4q1 * q3q3 - _2q3 * ax + 4.0f * q0q0 * q1 - _2q0 * ay - _4q1 + _8q1 * q1q1 + _8q1 * q2q2 + _4q1 * az;
                s2 = 4.0f * q0q0 * q2 + _2q0 * ax + _4q2 * q3q3 - _2q3 * ay - _4q2 + _8q2 * q1q1 + _8q2 * q2q2 + _4q2 * az;
                s3 = 4.0f * q1q1 * q3 - _2q1 * ax + 4.0f * q2q2 * q3 - _2q2 * ay;
                recipNorm = 1 / Math.Sqrt(s0 * s0 + s1 * s1 + s2 * s2 + s3 * s3); // normalise step magnitude
                s0 *= recipNorm;
                s1 *= recipNorm;
                s2 *= recipNorm;
                s3 *= recipNorm;

                // Apply feedback step
                qDot1 -= beta * s0;
                qDot2 -= beta * s1;
                qDot3 -= beta * s2;
                qDot4 -= beta * s3;
            }

            // Integrate rate of change of quaternion to yield quaternion
            q0 += qDot1 * (1.0f / sampleFreq);
            q1 += qDot2 * (1.0f / sampleFreq);
            q2 += qDot3 * (1.0f / sampleFreq);
            q3 += qDot4 * (1.0f / sampleFreq);

            // Normalise quaternion
            recipNorm = 1 / Math.Sqrt((q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3));
            q0 *= recipNorm;
            q1 *= recipNorm;
            q2 *= recipNorm;
            q3 *= recipNorm;
        }

        /// <summary>
        /// Calculates Linear Acceleration from Values provided from a CSV file
        /// </summary>
        /// <param name="values">String Array containing different values</param>
        /// <param name="lx">Output lx</param>
        /// <param name="ly">Output lx</param>
        /// <param name="lz">Output lx</param>
        private void CalculateLinear(String[] values, out double lx, out double ly, out double lz, out double Gx, out double Gy, out double Gz)
        {
            lx = ly = lz = 0;

            double CalcGravX, CalcGravY, CalcGravZ;
            
            Gx = double.Parse(values[GXCOL]);
            Gy = double.Parse(values[GYCOL]);
            Gz = double.Parse(values[GZCOL]);

            double Ax = double.Parse(values[AXCOL]);
            double Ay = double.Parse(values[AYCOL]);
            double Az = double.Parse(values[AZCOL]);

            double Mx = double.Parse(values[MXCOL]);
            double My = double.Parse(values[MYCOL]);
            double Mz = double.Parse(values[MZCOL]);

            MadgwickAHRSupdate(Gx, Gy, Gz, Ax, Ay, Az, Mx, My, Mz);
            GetGravity(q0, q1, q2, q3, out CalcGravX, out CalcGravY, out CalcGravZ);
            
            lx = Ax - (CalcGravX * 0.98);
            ly = Ay - (CalcGravY * 0.98);
            lz = Az - (CalcGravZ * 0.98);
        }


        /// <summary>
        /// Has to correct for orientation in the order Phoneview expects it.
        /// The order was set for MarkerParser as Ax, Ay, Az, Pitch*, Roll, Yaw*
        /// so Ax, Ay, Az, Gx, Gy, Gz need to be adjusted to match it
        /// </summary>
        /// <param name="DeviceID"></param>
        /// <param name="lx"></param>
        /// <param name="ly"></param>
        /// <param name="lz"></param>
        /// <param name="gx"></param>
        /// <param name="gy"></param>
        /// <param name="gz"></param>
        private void OrientAxes(Devices DeviceID, ref double lx, ref double ly, ref double lz, ref double gx, ref double gy, ref double gz)
        {
            double temp = 0.00;

            switch (DeviceID)
            {
                case Devices.Invensense:
                    /* Shimmer uses reversed directions for rotation and reverse direction for Z axis */
                    // Rotate Accelerations
                    lz = -lz;
                    // Rotate Gyroscope
                    gx = -gx;
                    gy = -gy;
                    break;
                case Devices.Actigraph:
                    // Rotate Acceleration
                    lz = lz * -1;
                    temp = -lx;
                    lx = ly;
                    ly = temp;
                    // Rotate Angular Velocities
                    gz = gz * -1;
                    temp = -gx;
                    gx = gy;
                    gy = temp;
                    break;
                default: break;
            }
        }


        /// <summary>
        /// Let the user pick a file from an open file dialog box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_PickFile_Click(object sender, EventArgs e)
        {
            // Create an instance of an OpenFileDialog
            OpenFileDialog ofn = new OpenFileDialog
            {
                // Set filter options and filter index.
                Filter = "CSV Files (.csv)|*.csv|All Files (*.*)|*.*",
                FilterIndex = 1
            };

            // Call the ShowDialog method to show the dialog box.
            DialogResult userClickedOK = ofn.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == DialogResult.OK)
            {
                TB_infile.Text = ofn.FileName;
                TB_outfile.Text = TB_infile.Text.Substring(0, TB_infile.Text.Length - 4) + ".act";

                lb_Status.Text = @"Ready";
                lb_Status.ForeColor = Color.Green;
                lb_Status.Refresh();
            }
        }
 
        /// <summary>
        /// This thread parses data from the csv file and writes an SHM file with linear acceleration and gyroscope data.
        /// The program hangs for a very long time if a thread is not used and confuses Windows (which asks if it should be terminated).
        /// </summary>
        private void WorkerThread()
        {
            q0 = 1;
            q1 = q2 = q3 = 0;

            long CurrentLine;

            UInt16 Percentage = 0;
            long TotalLines;
            double lx, ly, lz, Gx, Gy, Gz;
            String DateString;
            String TimeString;

            string line = "2018-01-03T09:24:00.0800000,-0.014648,-0.485840,0.915527,44.122773,-2.258301,14.099122,-77.392583,22.265624,-14.648437,-19.628905";
            var values = line.Split(separator);

            CurrentDevice = (Devices)System.Enum.Parse(typeof(Devices), "Actigraph");

            while (!EndThread)
            {
                runThread.WaitOne(Timeout.Infinite);

                while (!EndThread)
                {
                    try
                    {   
                        Percentage = 0;
                       
                        /* Modify the button so it can't be pressed again (avoids launching of multiple threads) */
                        lb_Status.Parent.Invoke((MethodInvoker)delegate {
                            lb_Status.Text = @"Working";
                            lb_Status.ForeColor = Color.YellowGreen;
                            lb_Status.Refresh();
                            BTN_go.Enabled = false;
                        });

                        reader = new StreamReader(File.OpenRead(inputFileName));
                        DataWriter = new BinaryWriter(File.OpenWrite(outputFileName));

                        string eventsfilename = outputFileName.Substring(0, outputFileName.Length - 5) + "-events.txt";
                        EventsWriter = new StreamWriter(File.OpenWrite(eventsfilename));

                        /* Read filesize to estimate progress */
                        FileInfo FI = new FileInfo(inputFileName);
                        TotalLines = (long) ((UInt64)FI.Length / (UInt64) BytesPerLine);

                        CurrentLine = 0;

                        Percentage =(UInt16)  (CurrentLine * 100 / TotalLines);


                        /* Read header lines with seperator information */
                        for(int i = 0; i < actigraphHeaderLines; i++)
                        {
                            line = reader.ReadLine();
                        }

                        /* Check if CSV column order matches */
                        if (line.CompareTo("Timestamp,Accelerometer X,Accelerometer Y,Accelerometer Z,Temperature,Gyroscope X,Gyroscope Y,Gyroscope Z,Magnetometer X,Magnetometer Y,Magnetometer Z") != 0)
                        {
                            lb_Status.Parent.Invoke((MethodInvoker)delegate
                            {
                                BTN_go.Enabled = true;
                                lb_Status.Text = "Number of columns in the CSV file do not match.";
                                lb_Status.ForeColor = Color.Red;
                            });
                            DataWriter.Close();
                            EndThread = true;
                            return;

                        }

                        /* Segment 1 */
                        CurrentLine = 0;

                        while (true)
                        {
                            line = reader.ReadLine();
                            if (reader.EndOfStream) break;

                            Percentage = (UInt16)(CurrentLine * 100 / TotalLines);

                            PB_convert.Parent.Invoke((MethodInvoker)delegate
                            {
                                PB_convert.ForeColor = Color.YellowGreen;
                                PB_convert.Value = ((UInt16)Percentage <= 100) ? (UInt16)Percentage : 100;
                            });

                            if (line.Substring(0, 10) != ExtractDateString)
                                continue;

                            /* Get Start Time from the text file */
                            if (datefound == false)
                            {
                                datefound = true;
                                values = line.Split(separator);
                                DateString = values[0].Substring(0, 10);
                                TimeString = values[0].Substring(11, 8);
                                EventsWriter.WriteLine("START\t" + DateString + "\t" + TimeString);
                            }

                            if((CurrentLine % 7) == 0)
                            {
                                values = line.Split(separator);

                                CalculateLinear(values, out lx, out ly, out lz, out Gx, out Gy, out Gz);
                                //OrientAxes(CurrentDevice, ref lx, ref ly, ref lz, ref Gx, ref Gy, ref Gz);

                                /* Shimmerview is expecting data in the following order: Ax, Ay, Az, Pitch*, Roll, Yaw* */
                                /* Pitch and Yaw positions don't matter, but Roll is used for features */
                                DataWriter.Write((float)lx);    /* Ax */
                                DataWriter.Write((float)ly);    /* Ay */
                                DataWriter.Write((float)lz);    /* Az */
                                DataWriter.Write((float)Gx);    /* Pitch */
                                DataWriter.Write((float)Gy);    /* Roll */
                                DataWriter.Write((float)Gz);    /* Yaw */
                            }

                            CurrentLine++;
                            if (CurrentLine == 100) CurrentLine = 0;
                        }

                        String Date = values[0].Substring(0, 10);
                        String Time = values[0].Substring(11, 8);
                        EventsWriter.WriteLine("END\t" + Date + "\t" + Time);

                        DataWriter.Close();
                        reader.Close();
                        EventsWriter.Close();

                        PB_convert.Parent.Invoke((MethodInvoker)delegate
                       {
                           PB_convert.ForeColor = Color.Green;
                           PB_convert.Value = 100;
                           lb_Status.Text = "Finished Reading File.";
                           lb_Status.ForeColor = Color.Green;
                           BTN_PickFile.Enabled = true;
                           lb_Status.Refresh();
                       });

                        if(datefound == false)
                        {
                            lb_Status.Parent.Invoke((MethodInvoker)delegate
                           {
                               lb_Status.Text = "Could not find the date you asked for in the file";
                               lb_Status.ForeColor = Color.Red;
                           });
                        }

                        EndThread = true;
                    }

                    catch (Exception ex)
                    {
                        lb_Status.Parent.Invoke((MethodInvoker)delegate {
                            BTN_go.Enabled = false;
                            lb_Status.Text = "An error has occured. Close this program and try again." + ex.ToString();
                            lb_Status.ForeColor = Color.Red;
                        });
                        DataWriter.Close();
                        reader.Close();
                        EventsWriter.Close();

                        EndThread = true;
                        return;
                    }
                }
            }
        }

        private void Btn_go_Click(object sender, EventArgs e)
        {
            ExtractDateString = dtp_dateTimePicker.Value.ToString("u").Substring(0, 10);
            inputFileName = TB_infile.Text;
            outputFileName = TB_outfile.Text;

            /* Check for errors in Filename */
            if( (TB_infile.Text.Equals("", StringComparison.Ordinal)) || (TB_outfile.Text.Equals("", StringComparison.Ordinal)) )
            {
                lb_Status.Parent.Invoke((MethodInvoker)delegate
                {
                    BTN_go.Enabled = true;
                    lb_Status.Text = "Please check input / output file paths";
                    lb_Status.ForeColor = Color.Red;
                });
            }
            else /* Run the thread to process data */
            {
                /* Create new thread and run */
                t = new Thread(WorkerThread);
                t.Start();
                /* Reset the thread */
                runThread.Set();
            }
        }
    }
}
