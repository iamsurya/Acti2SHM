using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Acti2SHM
{
    public partial class Frm_MainForm : Form
    {
        public Frm_MainForm()
        {
            InitializeComponent();
        }

        double q0, q1, q2, q3;
        double beta = 0.041;
        double sampleFreq = 15.0f;
        int TimeCOL, AXCOL, AYCOL, TempCOL, AZCOL, GXCOL, GYCOL, GZCOL, MXCOL, MYCOL, MZCOL; /* Column Numbers for the CSV */

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
        private void CalculateLinear(String[] values, out double lx, out double ly, out double lz)
        {
            lx = ly = lz = 0;

            double CalcGravX, CalcGravY, CalcGravZ;
            
            double Gx = double.Parse(values[GXCOL]);
            double Gy = double.Parse(values[GYCOL]);
            double Gz = double.Parse(values[GZCOL]);

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
                TB_IP_file.Text = ofn.FileName;
                String InputFile = TB_IP_file.Text;
                String OPFile = InputFile.Substring(0, InputFile.Length - 4) + ".acti";
                TB_outfile.Text = OPFile;

                lb_Status.Text = @"Ready";
                lb_Status.ForeColor = Color.Green;
                lb_Status.Refresh();
            }
        }

        private void Btn_go_Click(object sender, EventArgs e)
        {
            char SplitChar = ',';
            TimeCOL = 1; AXCOL = 2; AYCOL = 3; AZCOL = 4; TempCOL = 5; GXCOL = 6; GYCOL = 7; GZCOL = 8; MXCOL = 9; MYCOL = 10; MZCOL = 11;


        }
    }
}
