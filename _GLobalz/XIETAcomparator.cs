using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VC_WATERCRAFT._GLobalz
{
    public class XIETAcomparator
    {
        private double _shipLatitude;
        private double _shipLongitude;
        private double _waypointLatitude;
        private double _waypointLongitude;
        private double _shipHeading;
        private double radiusEarthEquatorialFt = 6378.14 * 100000 / (2.54 * 12); //20925600.787
        private double radiusEarthPolarFt = 6356.75 * 100000 / (2.54 * 12);     //20855462.992




        double[] offsetsMBIV_calcualted = new double[2];
        double[] offsets_matrixMath_calculated = new double[2];



        private double[] ComputeOffsetsXiEta(double delxFt, double delyFt, double headingDeg)
        {
            /*
             *  This function computes offset pair (xi, eta)
             *
             *  Note: xi  is stored in offsets[0]
             *        eta is stored in offsets[1]
             */

            // distance between actual position and commanded position (units: feet)
            double r_ft = Math.Sqrt(delxFt * delxFt + delyFt * delyFt);

            // alpha: the polar angle corresponding to the heading (range: 0 to 359.9999)
            double alpha_deg = 0;//= ConvertHeadingToPolar(heading_deg);
            if (headingDeg <= 90)
            {
                alpha_deg = 90 - headingDeg;
            }
            else if (headingDeg <= 360)
            {
                alpha_deg = 450 - headingDeg;
            }

            // beta: the angle of the vector between target point and vessel (range: 0 to 359.9999)
            double beta_deg = RadsToDegs(Math.Atan2(delyFt, delxFt));
            // Ensure that beta_deg >= 0 Note: Expected range of above expression: -180 to 180 deg
            if (beta_deg < 0) beta_deg += 360;

            double[] offsets = new double[2];

            if (beta_deg >= 0 && beta_deg <= 90)
            {   // vessel in 1ST QUADRANT
                double alphaMinusBeta_deg = alpha_deg - beta_deg;
                // ensure the following:  0 <= (alpha - beta) <= 360
                if (alphaMinusBeta_deg < 0) alphaMinusBeta_deg += 360;
                else if (alphaMinusBeta_deg > 360) alphaMinusBeta_deg -= 360;

                if (alphaMinusBeta_deg >= 0 && alphaMinusBeta_deg <= 90)
                {   // Case 1 (in notes)
                    offsets[0] = r_ft * Math.Sin(DegsToRads(alphaMinusBeta_deg)); // xi
                    offsets[1] = r_ft * Math.Cos(DegsToRads(alphaMinusBeta_deg)); // eta
                }
                else if (alphaMinusBeta_deg > 90 && alphaMinusBeta_deg <= 180)
                {   // Case 2 (in notes)
                    offsets[0] = r_ft * Math.Cos(DegsToRads(alphaMinusBeta_deg - 90)); // xi
                    offsets[1] = -r_ft * Math.Sin(DegsToRads(alphaMinusBeta_deg - 90)); // eta
                }
                else if (alphaMinusBeta_deg > 180 && alphaMinusBeta_deg <= 270)
                {   // Case 3 (in notes)
                    offsets[0] = -r_ft * Math.Sin(DegsToRads(alphaMinusBeta_deg - 180)); // xi
                    offsets[1] = -r_ft * Math.Cos(DegsToRads(alphaMinusBeta_deg - 180)); // eta
                }
                else if (alphaMinusBeta_deg > 270 && alphaMinusBeta_deg <= 360)
                {   // Case 4 (in notes)
                    offsets[0] = -r_ft * Math.Cos(DegsToRads(alphaMinusBeta_deg - 270)); // xi
                    offsets[1] = r_ft * Math.Sin(DegsToRads(alphaMinusBeta_deg - 270)); // eta
                }
            }
            else if (beta_deg > 90 && beta_deg <= 180)
            {   // vessel in 2ND QUADRANT
                double betaMinusAlpha_deg = beta_deg - alpha_deg;
                // ensure the following:  -180 <= (beta - alpha) <= 180
                if (betaMinusAlpha_deg < -180) betaMinusAlpha_deg += 360;
                else if (betaMinusAlpha_deg > 180) betaMinusAlpha_deg -= 360;

                if (betaMinusAlpha_deg <= 180 && betaMinusAlpha_deg >= 90)
                {   // Case 1 (in notes)
                    offsets[0] = -r_ft * Math.Cos(DegsToRads(betaMinusAlpha_deg - 90)); // xi
                    offsets[1] = -r_ft * Math.Sin(DegsToRads(betaMinusAlpha_deg - 90)); // eta
                }
                else if (betaMinusAlpha_deg < 90 && betaMinusAlpha_deg >= 0)
                {   // Case 2 (in notes)
                    offsets[0] = -r_ft * Math.Sin(DegsToRads(betaMinusAlpha_deg)); // xi
                    offsets[1] = r_ft * Math.Cos(DegsToRads(betaMinusAlpha_deg)); // eta
                }
                else if (betaMinusAlpha_deg < 0 && betaMinusAlpha_deg >= -90)
                {   // Case 3 (in notes)
                    offsets[0] = r_ft * Math.Cos(DegsToRads(betaMinusAlpha_deg + 90)); // xi
                    offsets[1] = r_ft * Math.Sin(DegsToRads(betaMinusAlpha_deg + 90)); // eta
                }
                else if (betaMinusAlpha_deg < -90 && betaMinusAlpha_deg >= -180)
                {   // Case 4 (in notes)
                    offsets[0] = r_ft * Math.Sin(DegsToRads(betaMinusAlpha_deg + 180)); // xi
                    offsets[1] = -r_ft * Math.Cos(DegsToRads(betaMinusAlpha_deg + 180)); // eta
                }
            }
            else if (beta_deg > 180 && beta_deg <= 270)
            {   // vessel in 3RD QUADRANT
                double alphaMinusBeta_deg = alpha_deg - beta_deg;
                // ensure the following:  -270 <= (alpha - beta) <= 90
                if (alphaMinusBeta_deg < -270) alphaMinusBeta_deg += 360;
                else if (alphaMinusBeta_deg > 90) alphaMinusBeta_deg -= 360;

                if (alphaMinusBeta_deg >= 0 && alphaMinusBeta_deg <= 90)
                {   // Case 3 (in notes)
                    offsets[0] = r_ft * Math.Sin(DegsToRads(alphaMinusBeta_deg)); // xi
                    offsets[1] = r_ft * Math.Cos(DegsToRads(alphaMinusBeta_deg)); // eta
                }
                else if (alphaMinusBeta_deg < 0 && alphaMinusBeta_deg >= -90)
                {   // Case 2 (in notes)
                    offsets[0] = -r_ft * Math.Cos(DegsToRads(alphaMinusBeta_deg + 90)); // xi
                    offsets[1] = r_ft * Math.Sin(DegsToRads(alphaMinusBeta_deg + 90)); // eta
                }
                else if (alphaMinusBeta_deg < -90 && alphaMinusBeta_deg >= -180)
                {   // Case 1 (in notes)
                    offsets[0] = -r_ft * Math.Sin(DegsToRads(alphaMinusBeta_deg + 180)); // xi
                    offsets[1] = -r_ft * Math.Cos(DegsToRads(alphaMinusBeta_deg + 180)); // eta
                }
                else if (alphaMinusBeta_deg < -180 && alphaMinusBeta_deg >= -270)
                {   // Case 4 (in notes)
                    offsets[0] = r_ft * Math.Cos(DegsToRads(alphaMinusBeta_deg + 270)); // xi
                    offsets[1] = -r_ft * Math.Sin(DegsToRads(alphaMinusBeta_deg + 270)); // eta
                }
            }
            else if (beta_deg > 270 && beta_deg <= 360)
            {   // vessel in 4TH QUADRANT
                double betaMinusAlpha_deg = beta_deg - alpha_deg;
                // ensure the following:  -90 <= (beta - alpha) <= 270
                if (betaMinusAlpha_deg < -90) betaMinusAlpha_deg += 360;
                else if (betaMinusAlpha_deg > 270) betaMinusAlpha_deg -= 360;

                if (betaMinusAlpha_deg >= -90 && betaMinusAlpha_deg <= 0)
                {   // Case 1 (in notes)
                    offsets[0] = r_ft * Math.Cos(DegsToRads(betaMinusAlpha_deg + 90)); // xi
                    offsets[1] = r_ft * Math.Sin(DegsToRads(betaMinusAlpha_deg + 90)); // eta
                }
                else if (betaMinusAlpha_deg > 0 && betaMinusAlpha_deg <= 90)
                {   // Case 4 (in notes)
                    offsets[0] = -r_ft * Math.Sin(DegsToRads(betaMinusAlpha_deg)); // xi
                    offsets[1] = r_ft * Math.Cos(DegsToRads(betaMinusAlpha_deg)); // eta
                }
                else if (betaMinusAlpha_deg > 90 && betaMinusAlpha_deg <= 180)
                {   // Case 3 (in notes)
                    offsets[0] = -r_ft * Math.Cos(DegsToRads(betaMinusAlpha_deg - 90)); // xi
                    offsets[1] = -r_ft * Math.Sin(DegsToRads(betaMinusAlpha_deg - 90)); // eta
                }
                else if (betaMinusAlpha_deg > 180 && betaMinusAlpha_deg <= 270)
                {   // Case 2 (in notes)
                    offsets[0] = r_ft * Math.Sin(DegsToRads(betaMinusAlpha_deg - 180)); // xi
                    offsets[1] = -r_ft * Math.Cos(DegsToRads(betaMinusAlpha_deg - 180)); // eta
                }
            }




            //double xi = offsets[0]/1000;
            //double eta = offsets[1]/ 1000;
            double xi = offsets[0];
            double eta = offsets[1];
            return new double[] { xi, eta };
        }
        private double DegsToRads(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        private double RadsToDegs(double radians)
        {
            return radians * (180.0 / Math.PI);
        }

        private void ComputeXiEtaValues()
        {
            double del_longitude_deg = _waypointLongitude - _shipLongitude;
            // Ensure that sign of del_longitude_deg is correct for any values of lon1 and lon2.
            // 	 Note: 	Range of del_longitude_deg is -180 to 180 deg
            //   		Of course, we expect the value of del_longitude_deg to be close to zero.
            if (_waypointLongitude > _shipLongitude)
            {
                if (del_longitude_deg > 180) del_longitude_deg -= 360;
            }
            else
            {
                if (del_longitude_deg < -180) del_longitude_deg += 360;
            }

            double delXFt = radiusEarthEquatorialFt * Math.Cos(_shipLatitude * Math.PI / 180) * del_longitude_deg * Math.PI / 180;

            double delYFt = radiusEarthPolarFt * (_waypointLatitude - _shipLatitude) * Math.PI / 180;

            // Compute Xi and Eta
            offsetsMBIV_calcualted = ComputeOffsetsXiEta(delXFt, delYFt, _shipHeading);




        }

        private void ComputeXiEtaValuesV2()
        {
            // Compute differences in latitude and longitude
            double dLat = _waypointLatitude - _shipLatitude;
            double dLon = _waypointLongitude - _shipLongitude;

            //364582.4828 ftperdeg
            double feetPerDegreeLat = 364582.4828;
            double feetPerDegreeLon = 364582.4828 * Math.Cos(_shipLatitude * Math.PI / 180);
            double dyFt = dLat * feetPerDegreeLat;
            double dxFt = dLon * feetPerDegreeLon;
            double headingRad = _shipHeading * Math.PI / 180;
            double cosHeading = Math.Cos(headingRad);
            double sinHeading = Math.Sin(headingRad);
            double xiFt = dxFt * cosHeading - dyFt * sinHeading;
            double etaFt = dxFt * sinHeading + dyFt * cosHeading;

            offsets_matrixMath_calculated[0] = xiFt;
            offsets_matrixMath_calculated[1] = etaFt;
        }


        public void UpdateParameters(double shipLat, double shipLon, double wpLat, double wpLon, double heading)
        {
            _shipLatitude = shipLat;
            _shipLongitude = shipLon;
            _waypointLatitude = wpLat;
            _waypointLongitude = wpLon;
            _shipHeading = heading;
            ComputeXiEtaValues();
            ComputeXiEtaValuesV2();
        }

        public double[] GetOffsetsMBIV()
        {
            return offsetsMBIV_calcualted;
        }

        public double[] GetOffsetsMatrixMath()
        {
            return offsets_matrixMath_calculated;
        }

        public XIETAcomparator() { }

        public double Get_radiusEarthEquatorialFt()
        {
            return radiusEarthEquatorialFt;
        }

    }
}
