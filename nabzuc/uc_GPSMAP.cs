using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using VC_WATERCRAFT._GLobalz;

namespace VC_WATERCRAFT
{
    public partial class uc_GPSMAP : UserControl
    {
        private XIETAcomparator _xietaComparator = new XIETAcomparator();
        private float _transverseDisplacement = 0f;
        private float _lateralDisplacement = 0f;
        private float _shipHeading = 0f;
        private const float GridSquaresPerEdge = 4f;
        private double _maxRangeUnits;
        private double _gridSquareSizeInPixels;
        private PointF _panelCenter;
        private bool _isDragging = false;
        private bool _unitsInMeters = false;
        private const double FeetPerMeter = 3.2808399;
        private double _arbiLatitude = 0.0;
        private double _arbiLongitude = 0.0;
        private float _arbiPointX;
        private float _arbiPointY;
        private double _greenDotLatitude;
        private double _greenDotLongitude;
        private PointF _greenDotPosition;
        private double _gridSquareSize = 1.0;
        private double _pixelsPerUnit; // FIX: Ensure this is defined as a class-level variable.

        private double _shipLatitude;
        private double _shipLongitude;
        private double _maxRange;
        private Label lblHeading;
        private TrackBar trackBarHeading;
        private CheckBox cb_lockArbit;
        private TextBox tx_ArbitDotRealCoordinates;
        private Label gridSizeLabel;
        private TextBox tx_GreenDotRealCoordinates;
        private CheckBox cb_mbivXIetaCalc;
        private Button btnSetPurpleToGreen;
        private Button btnCenterGreenDot;
        private TextBox tb_cneterLocDouble;
        private TextBox gridSquareSizeBox;
        private CheckBox unitsCheckBox;
        private DoubleBufferedPanel mapPanel2;

        public uc_GPSMAP()
        {
            CreateAndPlaceComponents();
            InitializeEvents();
        }

        private double GetEarthRadius()
        {
            double earthRadiusFeet = _xietaComparator.Get_radiusEarthEquatorialFt();
            if (_unitsInMeters)
            {
                return earthRadiusFeet / FeetPerMeter;
            }
            else
            {
                return earthRadiusFeet;
            }
        }


        //      0                          200                        300                  400
        // 0     tb_cneterLocDouble            unitsCheckBox          gridSquareSizeBox    gridSizeLabel
        // 40     tx_GreenDotRealCoordinates    cb_lockArbit           btnCenterGreenDot
        // 60     tx_ArbitDotRealCoordinates    cb_mbivXIetaCalc       btnSetPurpleToGreen
        // 80     mapPanel2
        // 100    trackBarHeading                                                           lblHeading 
        private void CreateAndPlaceComponents()
        {

            //set font size to 7 
            //this.Font = new Font("Arial", 7);
            // Setting up the initial coordinates and layout
            int startX = 2;
            int startY = 10;
            int verticalSpacing = 30;  
            int horizontalSpacing = 220;

            // 1st row
            tb_cneterLocDouble = new TextBox
            {
                Location = new Point(startX, startY),
                Width = 220,
                Text = "42.0021458, -73.0188745"
            };
            unitsCheckBox = new CheckBox
            {
                Location = new Point(startX + horizontalSpacing, startY),
                Text = "Units in Meters",
                Width = 120, // Set explicit width
                Height = 30  // Set explicit height
            };
            gridSquareSizeBox = new TextBox
            {
                Location = new Point(startX + horizontalSpacing * 2, startY),
                Width = 100,
                Text = "100"
            };
            gridSizeLabel = new Label
            {
                Location = new Point(startX + horizontalSpacing * 3, startY),
                Text = "sqr Size (ft):",
                Width = 150,  // Set explicit width
                Height = 30   // Set explicit height
            };

            // 2nd row
            tx_GreenDotRealCoordinates = new TextBox
            {
                Location = new Point(startX, startY + verticalSpacing),
                Width = 220,
                Text = "-",
                BackColor = Color.FromArgb(192, 255, 192)
            };
            cb_mbivXIetaCalc = new CheckBox
            {
                Location = new Point(startX + horizontalSpacing, startY + verticalSpacing),
                Text = "Use MBIV Calculation",
                Width = 130,  // Set explicit width
                Height = 30   // Set explicit height
            };
   
            btnCenterGreenDot = new Button
            {
                Location = new Point(startX + horizontalSpacing * 2, startY + verticalSpacing),
                Text = "Center Green Dot",
                Width = 120, // Adjusted button width
                Height = 30  // Adjusted button height
            };

            // 3rd row
            tx_ArbitDotRealCoordinates = new TextBox
            {
                Location = new Point(startX, startY + verticalSpacing * 2),
                Width = 220,
                Text = "-",
                BackColor = Color.FromArgb(192, 192, 255)
            };
       
            cb_lockArbit = new CheckBox
            {
                Location = new Point(startX + horizontalSpacing, startY + verticalSpacing * 2),
                Text = "Lock Arbitrary Point",
                Width = 120,  // Set explicit width
                Height = 30   // Set explicit height
            };
            btnSetPurpleToGreen = new Button
            {
                Location = new Point(startX + horizontalSpacing * 2, startY + verticalSpacing * 2),
                Text = "Set Purple to Green",
                Width = 120, // Adjusted button width
                Height = 30  // Adjusted button height
            };

            // 4th row

            mapPanel2 = new DoubleBufferedPanel
            {
                Location = new Point(startX, startY + verticalSpacing * 3),
                Size = new Size(601, 601),
                BackColor = Color.White
            };

            // 5th row
  
            trackBarHeading = new TrackBar
            {
                Location = new Point(startX, startY + verticalSpacing * 3 + 611),
                Width = 240,
                Minimum = 0,
                Maximum = 359,
                TickFrequency = 45,
                TickStyle = TickStyle.BottomRight
            };
            lblHeading = new Label
            {
                Location = new Point(startX + 250, startY + verticalSpacing * 3+ 611),
                Text = "Heading: 0°",
                Width = 100,  // Set explicit width
                Height = 25   // Set explicit height
            };

            // Adding controls to the form
            this.Controls.Add(tb_cneterLocDouble);
            this.Controls.Add(unitsCheckBox);
            this.Controls.Add(gridSquareSizeBox);
            this.Controls.Add(gridSizeLabel);
            this.Controls.Add(tx_GreenDotRealCoordinates);
            this.Controls.Add(cb_lockArbit);
            this.Controls.Add(btnCenterGreenDot);
            this.Controls.Add(tx_ArbitDotRealCoordinates);
            this.Controls.Add(cb_mbivXIetaCalc);
            this.Controls.Add(btnSetPurpleToGreen);
            this.Controls.Add(trackBarHeading);
            this.Controls.Add(lblHeading);
            this.Controls.Add(mapPanel2);

            // Adjust form size
            this.Width = 604;
            this.Height = 760;
        }

        private void InitializeEvents()
        {
            trackBarHeading.ValueChanged += TrackBarHeading_ValueChanged;
            btnCenterGreenDot.Click += btn_centerGreendot_Click;
            btnSetPurpleToGreen.Click += Btn_setPurpToGreen_Click;
            unitsCheckBox.CheckedChanged += UnitsCheckBox_CheckedChanged;
            cb_lockArbit.CheckedChanged += CbLockArbit_CheckedChanged;
            mapPanel2.Paint += MapPanel_Paint;
            mapPanel2.MouseDown += MapPanel_MouseDown;
            mapPanel2.MouseMove += MapPanel_MouseMove;
            mapPanel2.MouseUp += MapPanel_MouseUp;

            tb_cneterLocDouble.TextChanged += Tb_centerLocDouble_TextChanged;
            gridSquareSizeBox.TextChanged += (s, e) =>
            {
                UpdateArbitraryPoint();
                mapPanel2.Invalidate();
            };

            cb_mbivXIetaCalc.CheckedChanged += (s, e) =>
            {
                UpdateArbitraryPoint();
                mapPanel2.Invalidate();
            };
            tx_ArbitDotRealCoordinates.TextChanged += tx_ArbitDotRealCoordinatesTextChanged;
            cb_lockArbit.CheckedChanged += Cb_lockArbit_CheckedChanged;
            _greenDotPosition = new PointF(mapPanel2.Width / 2f, mapPanel2.Height / 2f);
        }

        private void TrackBarHeading_ValueChanged(object sender, EventArgs e)
        {
            //_shipHeading = trackBarHeading.Value;
            //mapPanel2.Invalidate();
            //lblHeading.Text = $"Heading: {_shipHeading}°";
               int _headingValue = trackBarHeading.Value % 360;
            lblHeading.Text = _headingValue.ToString();
            _shipHeading = _headingValue;
            UpdateDisplacements();
            UpdateArbitraryPoint();
            mapPanel2.Invalidate();
        }

        private void btn_centerGreendot_Click(object sender, EventArgs e)
        {
            float centerX = mapPanel2.Width / 2f;
            float centerY = mapPanel2.Height / 2f;
            _greenDotPosition = new PointF(centerX, centerY);
            UpdateDisplacements();
            mapPanel2.Invalidate();
        }

        private void Btn_setPurpToGreen_Click(object sender, EventArgs e)
        {
            _arbiLatitude = _greenDotLatitude;
            _arbiLongitude = _greenDotLongitude;
            tx_ArbitDotRealCoordinates.Text = $"{_arbiLatitude:F6}, {_arbiLongitude:F6}";
            UpdateArbitraryPoint();
            mapPanel2.Invalidate();
        }

        private void UnitsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _unitsInMeters = unitsCheckBox.Checked;
            gridSizeLabel.Text = _unitsInMeters ? "sqr Size (m)" : "sqr Size (ft)";
            UpdateDisplacements();
            UpdateArbitraryPoint();
            mapPanel2.Invalidate();
        }

        private void CbLockArbit_CheckedChanged(object sender, EventArgs e)
        {
            UpdateArbitraryPoint();
            UpdateDisplacements();
            mapPanel2.Invalidate();
        }

        private void MapPanel_MouseDown(object sender, MouseEventArgs e)
        {
            float dx = e.X - _greenDotPosition.X;
            float dy = e.Y - _greenDotPosition.Y;
            float distanceSquared = dx * dx + dy * dy;
            float dotRadius = 5f;
            if (distanceSquared <= dotRadius * dotRadius)
            {
                _isDragging = true;
            }
        }

        private void MapPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                _greenDotPosition = new PointF(e.X, e.Y);
                UpdateDisplacements();
                mapPanel2.Invalidate();
            }
        }
        private void UpdateMaxRange()
        {
            _maxRangeUnits = _gridSquareSize * GridSquaresPerEdge / 2.0;
            _pixelsPerUnit = mapPanel2.Width / (GridSquaresPerEdge * _gridSquareSize);
            _maxRange = (_gridSquareSize * GridSquaresPerEdge) / 2.0;
        }
        private void MapPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
            }
        }
        private void MapPanel_Paintold(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(mapPanel2.BackColor);
            PointF targetPoint = GetCurrentTargetPoint();
            if (!double.TryParse(gridSquareSizeBox.Text, out _gridSquareSize) || _gridSquareSize <= 0)
            {
                _gridSquareSize = 1.0;
            }
            double gridSquareSizeUnits = _gridSquareSize;
            double gridSquareSizeInPixels = mapPanel2.Width / GridSquaresPerEdge;

            _gridSquareSizeInPixels = mapPanel2.Width / GridSquaresPerEdge;
            _pixelsPerUnit = _gridSquareSizeInPixels / _gridSquareSize; // Pixels per physical unit (meters/feet)

 
            UpdateMaxRange();
            float centerX = mapPanel2.Width / 2f;
            float centerY = mapPanel2.Height / 2f;
            PointF centerPoint = new PointF(centerX, centerY);
            _panelCenter = centerPoint;
            DrawGrid(e.Graphics, gridSquareSizeInPixels);
            DrawRangeRings(e.Graphics);
            float centerDotRadius = 3f;
            using (SolidBrush redBrush = new SolidBrush(Color.Red))
            {
                e.Graphics.FillEllipse(redBrush, centerX - centerDotRadius, centerY - centerDotRadius, centerDotRadius * 2, centerDotRadius * 2);
            }
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            string unitLabel = _unitsInMeters ? "m" : "ft";
            float pixelsPerUnit = (float)_pixelsPerUnit;
            float angleRadians = _shipHeading * (float)(Math.PI / 180f);
            float shipSize = 20f;
            float frontX = centerX + shipSize * (float)Math.Sin(angleRadians);
            float frontY = centerY - shipSize * (float)Math.Cos(angleRadians);
            float rearLeftAngle = angleRadians + (float)(135 * Math.PI / 180f);
            float rearLeftX = centerX + shipSize * 0.5f * (float)Math.Sin(rearLeftAngle);
            float rearLeftY = centerY - shipSize * 0.5f * (float)Math.Cos(rearLeftAngle);
            float rearRightAngle = angleRadians - (float)(135 * Math.PI / 180f);
            float rearRightX = centerX + shipSize * 0.5f * (float)Math.Sin(rearRightAngle);
            float rearRightY = centerY - shipSize * 0.5f * (float)Math.Cos(rearRightAngle);
            PointF[] shipPoints = new PointF[]
            {
        new PointF(frontX, frontY),
        new PointF(rearLeftX, rearLeftY),
        new PointF(rearRightX, rearRightY)
            };
            using (SolidBrush shipBrush = new SolidBrush(Color.Blue))
            {
                e.Graphics.FillPolygon(shipBrush, shipPoints);
            }
            using (Pen shipPen = new Pen(Color.Black))
            {
                e.Graphics.DrawPolygon(shipPen, shipPoints);
            }
            float transverseDisplacementPixels = _transverseDisplacement * pixelsPerUnit;
            float endX = centerX + transverseDisplacementPixels * (float)Math.Sin(angleRadians);
            float endY = centerY - transverseDisplacementPixels * (float)Math.Cos(angleRadians);
            PointF endPoint = new PointF(endX, endY);
            using (Pen bluePen = new Pen(Color.Blue, 2))
            {
                e.Graphics.DrawLine(bluePen, centerPoint, endPoint);
            }
            string transverseDistanceText = $"{_transverseDisplacement:F2} {unitLabel}";
            float midXTransverse = (centerX + endX) / 2;
            float midYTransverse = (centerY + endY) / 2;
            SizeF textSizeTransverse = e.Graphics.MeasureString(transverseDistanceText, this.Font);
            float textPosXTransverse = midXTransverse - textSizeTransverse.Width / 2;
            float textPosYTransverse = midYTransverse - textSizeTransverse.Height - 5;
            RectangleF textBackgroundTransverse = new RectangleF(textPosXTransverse, textPosYTransverse, textSizeTransverse.Width, textSizeTransverse.Height);
            using (SolidBrush backgroundBrush = new SolidBrush(Color.FromArgb(200, Color.White)))
            {
                e.Graphics.FillRectangle(backgroundBrush, textBackgroundTransverse);
            }
            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                e.Graphics.DrawString(transverseDistanceText, this.Font, textBrush, textPosXTransverse, textPosYTransverse);
            }
            if (_lateralDisplacement != 0)
            {
                float perpendicularAngleRadians = angleRadians + ((_lateralDisplacement >= 0) ? (float)(Math.PI / 2) : (float)(-Math.PI / 2));
                float lateralDisplacementPixels = Math.Abs(_lateralDisplacement) * pixelsPerUnit;
                float lateralEndX = endX + lateralDisplacementPixels * (float)Math.Sin(perpendicularAngleRadians);
                float lateralEndY = endY - lateralDisplacementPixels * (float)Math.Cos(perpendicularAngleRadians);
                PointF lateralEndPoint = new PointF(lateralEndX, lateralEndY);
                using (Pen cyanPen = new Pen(Color.Cyan, 2))
                {
                    e.Graphics.DrawLine(cyanPen, endPoint, lateralEndPoint);
                }
                string lateralDistanceText = $"{_lateralDisplacement:F2} {unitLabel}";
                float midXLateral = (endX + lateralEndX) / 2;
                float midYLateral = (endY + lateralEndY) / 2;
                SizeF textSizeLateral = e.Graphics.MeasureString(lateralDistanceText, this.Font);
                float textPosXLateral = midXLateral - textSizeLateral.Width / 2;
                float textPosYLateral = midYLateral - textSizeLateral.Height - 5;
                RectangleF textBackgroundLateral = new RectangleF(textPosXLateral, textPosYLateral, textSizeLateral.Width, textSizeLateral.Height);
                using (SolidBrush backgroundBrush = new SolidBrush(Color.FromArgb(200, Color.White)))
                {
                    e.Graphics.FillRectangle(backgroundBrush, textBackgroundLateral);
                }
                using (SolidBrush textBrush = new SolidBrush(Color.Black))
                {
                    e.Graphics.DrawString(lateralDistanceText, this.Font, textBrush, textPosXLateral, textPosYLateral);
                }
            }
            float dotRadius = 5f;
            using (SolidBrush greenBrush = new SolidBrush(Color.Green))
            {
                e.Graphics.FillEllipse(greenBrush, _greenDotPosition.X - dotRadius, _greenDotPosition.Y - dotRadius, dotRadius * 2, dotRadius * 2);
            }
            float arbiDotRadius = dotRadius - 1f;
            using (SolidBrush purpleBrush = new SolidBrush(Color.Purple))
            {
                e.Graphics.FillEllipse(purpleBrush, _arbiPointX - arbiDotRadius, _arbiPointY - arbiDotRadius, arbiDotRadius * 2, arbiDotRadius * 2);
            }
            using (Pen yellowPen = new Pen(Color.Yellow, 1))
            {
                e.Graphics.DrawLine(yellowPen, centerPoint, targetPoint);
            }
            float deltaX = targetPoint.X - centerX;
            float deltaY = targetPoint.Y - centerY;
            float pixelDistance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            float realDistanceUnits = pixelDistance / pixelsPerUnit;
            string distanceText = $"{realDistanceUnits:F2} {unitLabel}";
            SizeF textSize = e.Graphics.MeasureString(distanceText, this.Font);
            float textPosX = centerX + deltaX / 2 - textSize.Width / 2;
            float textPosY = centerY + deltaY / 2 - textSize.Height / 2;
            RectangleF textBackground = new RectangleF(textPosX, textPosY, textSize.Width, textSize.Height);
            using (SolidBrush backgroundBrush = new SolidBrush(Color.FromArgb(200, Color.White)))
            {
                e.Graphics.FillRectangle(backgroundBrush, textBackground);
            }
            float maxRangePixels = (float)(_maxRange * pixelsPerUnit);
            float outerRadius = maxRangePixels;
            float innerRadius = outerRadius / 2f;
            float outerRingRadiusUnits = outerRadius / (float)pixelsPerUnit;
            float innerRingRadiusUnits = innerRadius / (float)pixelsPerUnit;
            string outerRingLabel = $"O: {outerRingRadiusUnits:F2} {unitLabel}";
            string innerRingLabel = $"I: {innerRingRadiusUnits:F2} {unitLabel}";
            float labelMargin = 10f;
            SizeF outerLabelSize = e.Graphics.MeasureString(outerRingLabel, this.Font);
            SizeF innerLabelSize = e.Graphics.MeasureString(innerRingLabel, this.Font);
            float outerLabelPosX = centerX - outerLabelSize.Width / 2f;
            float outerLabelPosY = labelMargin;
            float innerLabelPosX = centerX - innerLabelSize.Width / 2f;
            float innerLabelPosY = _panelCenter.Y - innerRadius - innerLabelSize.Height - labelMargin;
            if (innerLabelPosY < outerLabelPosY + outerLabelSize.Height + labelMargin)
            {
                innerLabelPosY = outerLabelPosY + outerLabelSize.Height + labelMargin;
            }
            using (SolidBrush backgroundBrush = new SolidBrush(Color.FromArgb(200, Color.White)))
            {
                e.Graphics.FillRectangle(backgroundBrush, outerLabelPosX, outerLabelPosY, outerLabelSize.Width, outerLabelSize.Height);
                e.Graphics.FillRectangle(backgroundBrush, innerLabelPosX, innerLabelPosY, innerLabelSize.Width, innerLabelSize.Height);
            }
            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                e.Graphics.DrawString(outerRingLabel, this.Font, textBrush, outerLabelPosX, outerLabelPosY);
                e.Graphics.DrawString(innerRingLabel, this.Font, textBrush, innerLabelPosX, innerLabelPosY);
            }
            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                e.Graphics.DrawString(distanceText, this.Font, textBrush, textPosX, textPosY);
            }
            using (SolidBrush cornerBrush = new SolidBrush(Color.Magenta))
            {
                float totalGridSize = (float)(GridSquaresPerEdge * gridSquareSizeInPixels);
                float gridOriginX = (mapPanel2.ClientSize.Width - totalGridSize) / 2f;
                float gridOriginY = (mapPanel2.ClientSize.Height - totalGridSize) / 2f;
                e.Graphics.FillEllipse(cornerBrush, gridOriginX - 2, gridOriginY - 2, 4, 4);
                e.Graphics.FillEllipse(cornerBrush, gridOriginX + totalGridSize - 2, gridOriginY - 2, 4, 4);
                e.Graphics.FillEllipse(cornerBrush, gridOriginX - 2, gridOriginY + totalGridSize - 2, 4, 4);
                e.Graphics.FillEllipse(cornerBrush, gridOriginX + totalGridSize - 2, gridOriginY + totalGridSize - 2, 4, 4);
            }
        }

        private void MapPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(mapPanel2.BackColor);
            PointF targetPoint = GetCurrentTargetPoint();
            if (!double.TryParse(gridSquareSizeBox.Text, out _gridSquareSize) || _gridSquareSize <= 0)
            {
                _gridSquareSize = 1.0;
            }
            double gridSquareSizeUnits = _gridSquareSize;
            double gridSquareSizeInPixels = mapPanel2.Width / GridSquaresPerEdge;
            _pixelsPerUnit = gridSquareSizeInPixels / gridSquareSizeUnits;
            UpdateMaxRange();
            float centerX = mapPanel2.Width / 2f;
            float centerY = mapPanel2.Height / 2f;
            PointF centerPoint = new PointF(centerX, centerY);
            _panelCenter = centerPoint;
            DrawGrid(e.Graphics, gridSquareSizeInPixels);
            DrawRangeRings(e.Graphics);
            float centerDotRadius = 3f;
            using (SolidBrush redBrush = new SolidBrush(Color.Red))
            {
                e.Graphics.FillEllipse(redBrush, centerX - centerDotRadius, centerY - centerDotRadius, centerDotRadius * 2, centerDotRadius * 2);
            }
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            string unitLabel = _unitsInMeters ? "m" : "ft";
            float pixelsPerUnit = (float)_pixelsPerUnit;
            float angleRadians = _shipHeading * (float)(Math.PI / 180f);
            float shipSize = 20f;
            float frontX = centerX + shipSize * (float)Math.Sin(angleRadians);
            float frontY = centerY - shipSize * (float)Math.Cos(angleRadians);
            float rearLeftAngle = angleRadians + (float)(135 * Math.PI / 180f);
            float rearLeftX = centerX + shipSize * 0.5f * (float)Math.Sin(rearLeftAngle);
            float rearLeftY = centerY - shipSize * 0.5f * (float)Math.Cos(rearLeftAngle);
            float rearRightAngle = angleRadians - (float)(135 * Math.PI / 180f);
            float rearRightX = centerX + shipSize * 0.5f * (float)Math.Sin(rearRightAngle);
            float rearRightY = centerY - shipSize * 0.5f * (float)Math.Cos(rearRightAngle);
            PointF[] shipPoints = new PointF[]
            {
        new PointF(frontX, frontY),
        new PointF(rearLeftX, rearLeftY),
        new PointF(rearRightX, rearRightY)
            };
            using (SolidBrush shipBrush = new SolidBrush(Color.Blue))
            {
                e.Graphics.FillPolygon(shipBrush, shipPoints);
            }
            using (Pen shipPen = new Pen(Color.Black))
            {
                e.Graphics.DrawPolygon(shipPen, shipPoints);
            }
            float transverseDisplacementPixels = _transverseDisplacement * pixelsPerUnit;
            float endX = centerX + transverseDisplacementPixels * (float)Math.Sin(angleRadians);
            float endY = centerY - transverseDisplacementPixels * (float)Math.Cos(angleRadians);
            PointF endPoint = new PointF(endX, endY);
            using (Pen bluePen = new Pen(Color.Blue, 2))
            {
                e.Graphics.DrawLine(bluePen, centerPoint, endPoint);
            }
            string transverseDistanceText = $"{_transverseDisplacement:F2} {unitLabel}";
            float midXTransverse = (centerX + endX) / 2;
            float midYTransverse = (centerY + endY) / 2;
            SizeF textSizeTransverse = e.Graphics.MeasureString(transverseDistanceText, this.Font);
            float textPosXTransverse = midXTransverse - textSizeTransverse.Width / 2;
            float textPosYTransverse = midYTransverse - textSizeTransverse.Height - 5;
            RectangleF textBackgroundTransverse = new RectangleF(textPosXTransverse, textPosYTransverse, textSizeTransverse.Width, textSizeTransverse.Height);
            using (SolidBrush backgroundBrush = new SolidBrush(Color.FromArgb(200, Color.White)))
            {
                e.Graphics.FillRectangle(backgroundBrush, textBackgroundTransverse);
            }
            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                e.Graphics.DrawString(transverseDistanceText, this.Font, textBrush, textPosXTransverse, textPosYTransverse);
            }
            if (_lateralDisplacement != 0)
            {
                float perpendicularAngleRadians = angleRadians + ((_lateralDisplacement >= 0) ? (float)(Math.PI / 2) : (float)(-Math.PI / 2));
                float lateralDisplacementPixels = Math.Abs(_lateralDisplacement) * pixelsPerUnit;
                float lateralEndX = endX + lateralDisplacementPixels * (float)Math.Sin(perpendicularAngleRadians);
                float lateralEndY = endY - lateralDisplacementPixels * (float)Math.Cos(perpendicularAngleRadians);
                PointF lateralEndPoint = new PointF(lateralEndX, lateralEndY);
                using (Pen cyanPen = new Pen(Color.Cyan, 2))
                {
                    e.Graphics.DrawLine(cyanPen, endPoint, lateralEndPoint);
                }
                string lateralDistanceText = $"{_lateralDisplacement:F2} {unitLabel}";
                float midXLateral = (endX + lateralEndX) / 2;
                float midYLateral = (endY + lateralEndY) / 2;
                SizeF textSizeLateral = e.Graphics.MeasureString(lateralDistanceText, this.Font);
                float textPosXLateral = midXLateral - textSizeLateral.Width / 2;
                float textPosYLateral = midYLateral - textSizeLateral.Height - 5;
                RectangleF textBackgroundLateral = new RectangleF(textPosXLateral, textPosYLateral, textSizeLateral.Width, textSizeLateral.Height);
                using (SolidBrush backgroundBrush = new SolidBrush(Color.FromArgb(200, Color.White)))
                {
                    e.Graphics.FillRectangle(backgroundBrush, textBackgroundLateral);
                }
                using (SolidBrush textBrush = new SolidBrush(Color.Black))
                {
                    e.Graphics.DrawString(lateralDistanceText, this.Font, textBrush, textPosXLateral, textPosYLateral);
                }
            }
            float dotRadius = 5f;
            using (SolidBrush greenBrush = new SolidBrush(Color.Green))
            {
                e.Graphics.FillEllipse(greenBrush, _greenDotPosition.X - dotRadius, _greenDotPosition.Y - dotRadius, dotRadius * 2, dotRadius * 2);
            }
            float arbiDotRadius = dotRadius - 1f;
            using (SolidBrush purpleBrush = new SolidBrush(Color.Purple))
            {
                e.Graphics.FillEllipse(purpleBrush, _arbiPointX - arbiDotRadius, _arbiPointY - arbiDotRadius, arbiDotRadius * 2, arbiDotRadius * 2);
            }
            using (Pen yellowPen = new Pen(Color.Yellow, 1))
            {
                e.Graphics.DrawLine(yellowPen, centerPoint, targetPoint);
            }
            float deltaX = targetPoint.X - centerX;
            float deltaY = targetPoint.Y - centerY;
            float pixelDistance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            float realDistanceUnits = pixelDistance / pixelsPerUnit;
            string distanceText = $"{realDistanceUnits:F2} {unitLabel}";
            SizeF textSize = e.Graphics.MeasureString(distanceText, this.Font);
            float textPosX = centerX + deltaX / 2 - textSize.Width / 2;
            float textPosY = centerY + deltaY / 2 - textSize.Height / 2;
            RectangleF textBackground = new RectangleF(textPosX, textPosY, textSize.Width, textSize.Height);
            using (SolidBrush backgroundBrush = new SolidBrush(Color.FromArgb(200, Color.White)))
            {
                e.Graphics.FillRectangle(backgroundBrush, textBackground);
            }
            float maxRangePixels = (float)(_maxRange * _pixelsPerUnit);
            float outerRadius = maxRangePixels;
            float innerRadius = outerRadius / 2f;
            float outerRingRadiusUnits = outerRadius / (float)_pixelsPerUnit;
            float innerRingRadiusUnits = innerRadius / (float)_pixelsPerUnit;
            string outerRingLabel = $"O: {outerRingRadiusUnits:F2} {unitLabel}";
            string innerRingLabel = $"I: {innerRingRadiusUnits:F2} {unitLabel}";
            float labelMargin = 10f;
            SizeF outerLabelSize = e.Graphics.MeasureString(outerRingLabel, this.Font);
            SizeF innerLabelSize = e.Graphics.MeasureString(innerRingLabel, this.Font);
            float outerLabelPosX = centerX - outerLabelSize.Width / 2f;
            float outerLabelPosY = labelMargin;
            float innerLabelPosX = centerX - innerLabelSize.Width / 2f;
            float innerLabelPosY = _panelCenter.Y - innerRadius - innerLabelSize.Height - labelMargin;
            if (innerLabelPosY < outerLabelPosY + outerLabelSize.Height + labelMargin)
            {
                innerLabelPosY = outerLabelPosY + outerLabelSize.Height + labelMargin;
            }
            using (SolidBrush backgroundBrush = new SolidBrush(Color.FromArgb(200, Color.White)))
            {
                e.Graphics.FillRectangle(backgroundBrush, outerLabelPosX, outerLabelPosY, outerLabelSize.Width, outerLabelSize.Height);
                e.Graphics.FillRectangle(backgroundBrush, innerLabelPosX, innerLabelPosY, innerLabelSize.Width, innerLabelSize.Height);
            }
            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                e.Graphics.DrawString(outerRingLabel, this.Font, textBrush, outerLabelPosX, outerLabelPosY);
                e.Graphics.DrawString(innerRingLabel, this.Font, textBrush, innerLabelPosX, innerLabelPosY);
            }
            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                e.Graphics.DrawString(distanceText, this.Font, textBrush, textPosX, textPosY);
            }
            using (SolidBrush cornerBrush = new SolidBrush(Color.Magenta))
            {
                float totalGridSize = (float)(GridSquaresPerEdge * gridSquareSizeInPixels);
                float gridOriginX = (mapPanel2.ClientSize.Width - totalGridSize) / 2f;
                float gridOriginY = (mapPanel2.ClientSize.Height - totalGridSize) / 2f;
                e.Graphics.FillEllipse(cornerBrush, gridOriginX - 2, gridOriginY - 2, 4, 4);
                e.Graphics.FillEllipse(cornerBrush, gridOriginX + totalGridSize - 2, gridOriginY - 2, 4, 4);
                e.Graphics.FillEllipse(cornerBrush, gridOriginX - 2, gridOriginY + totalGridSize - 2, 4, 4);
                e.Graphics.FillEllipse(cornerBrush, gridOriginX + totalGridSize - 2, gridOriginY + totalGridSize - 2, 4, 4);
            }
        }

        public void SetShipLocation(double lat, double lon)
        {
            _shipLatitude = lat;
            _shipLongitude = lon;
            UpdateArbitraryPoint();
            mapPanel2.Invalidate();
        }

        private void DrawGrid(Graphics g, double arggridSquareSizeInPixels)
        {

            float totalGridSize = (float)(GridSquaresPerEdge * arggridSquareSizeInPixels);
            
            float gridOriginX = (_panelCenter.X - totalGridSize / 2f);
            float gridOriginY = (_panelCenter.Y - totalGridSize / 2f);
            gridOriginX = (float)Math.Round(gridOriginX);
            gridOriginY = (float)Math.Round(gridOriginY);
            using (Pen gridPen = new Pen(Color.LightGray))
            {
                for (int i = 0; i <= 4; i++)
                {
                    float offset = i * (float)arggridSquareSizeInPixels;
                    g.DrawLine(gridPen, gridOriginX + offset, gridOriginY, gridOriginX + offset, gridOriginY + totalGridSize);
                    g.DrawLine(gridPen, gridOriginX, gridOriginY + offset, gridOriginX + totalGridSize, gridOriginY + offset);
                }
            }
        }
 

        private void DrawRangeRings(Graphics g)
        {
            float panelSize = Math.Min(mapPanel2.Width, mapPanel2.Height);
            float outerRadius = panelSize / 2f;
            float innerRadius = outerRadius / 2f;

            using (Pen ringPen = new Pen(Color.Black, 1))
            {
                g.DrawEllipse(ringPen, _panelCenter.X - outerRadius, _panelCenter.Y - outerRadius, outerRadius * 2, outerRadius * 2);
                g.DrawEllipse(ringPen, _panelCenter.X - innerRadius, _panelCenter.Y - innerRadius, innerRadius * 2, innerRadius * 2);
            }
        }

        private PointF GetCurrentTargetPoint()
        {
            if (cb_lockArbit.Checked)
            {
                return new PointF(_arbiPointX, _arbiPointY);
            }
            else
            {
                return _greenDotPosition;
            }
        }

        private void UpdateArbitraryPoint()
        {
            _xietaComparator.UpdateParameters(_shipLatitude, _shipLongitude, _arbiLatitude, _arbiLongitude, _shipHeading);
            double[] offsets;
            bool useMBIV = cb_mbivXIetaCalc.Checked;
            if (useMBIV)
            {
                offsets = _xietaComparator.GetOffsetsMBIV();
            }
            else
            {
                offsets = _xietaComparator.GetOffsetsMatrixMath();
            }
            double xiArbi = offsets[0];
            double etaArbi = offsets[1];
            if (_unitsInMeters)
            {
                xiArbi /= FeetPerMeter;
                etaArbi /= FeetPerMeter;
            }
            float xiUnits = (float)xiArbi;
            float etaUnits = (float)etaArbi;
            float xiPixels = xiUnits * (float)_pixelsPerUnit;
            float etaPixels = etaUnits * (float)_pixelsPerUnit;
            float centerX = mapPanel2.Width / 2f;
            float centerY = mapPanel2.Height / 2f;
            float angleRadians = _shipHeading * (float)(Math.PI / 180f);
            float deltaX = etaPixels * (float)Math.Sin(angleRadians) + xiPixels * (float)Math.Cos(angleRadians);
            float deltaY = -etaPixels * (float)Math.Cos(angleRadians) + xiPixels * (float)Math.Sin(angleRadians);
            _arbiPointX = centerX + deltaX;
            _arbiPointY = centerY + deltaY;
        }

        private void UpdateDisplacements()
        {
            PointF targetPoint = GetCurrentTargetPoint();
            float deltaX = targetPoint.X - mapPanel2.Width / 2f;
            float deltaY = mapPanel2.Height / 2f - targetPoint.Y;
            float displacementXUnits = deltaX / (float)_pixelsPerUnit;
            float displacementYUnits = deltaY / (float)_pixelsPerUnit;
            ConvertDisplacementToLatLon(displacementXUnits, displacementYUnits, out double pointLatitude, out double pointLongitude);
            if (!cb_lockArbit.Checked)
            {
                _greenDotLatitude = pointLatitude;
                _greenDotLongitude = pointLongitude;
            }
            // lbl_pointLat.Text = pointLatitude.ToString("F6", CultureInfo.InvariantCulture);
            // lbl_pointLon.Text = pointLongitude.ToString("F6", CultureInfo.InvariantCulture);
            tx_GreenDotRealCoordinates.Text = $"{pointLatitude:F6}, {pointLongitude:F6}";
            _xietaComparator.UpdateParameters(_shipLatitude, _shipLongitude, pointLatitude, pointLongitude, _shipHeading);
            double[] offsets;
            bool useMBIV = cb_mbivXIetaCalc.Checked;
            if (useMBIV)
            {
                offsets = _xietaComparator.GetOffsetsMBIV();
            }
            else
            {
                offsets = _xietaComparator.GetOffsetsMatrixMath();
            }
            double xi = offsets[0];
            double eta = offsets[1];
            if (_unitsInMeters)
            {
                xi /= FeetPerMeter;
                eta /= FeetPerMeter;
            }
            _lateralDisplacement = (float)xi;
            _transverseDisplacement = (float)eta;
            //  tb_TranverseDisplacement.Text = _transverseDisplacement.ToString("F2", CultureInfo.InvariantCulture);
            // tb_LateralDisplacement.Text = _lateralDisplacement.ToString("F2", CultureInfo.InvariantCulture);
            if (cb_lockArbit.Checked)
            {
                UpdateArbitraryPoint();
            }
            mapPanel2.Invalidate();
        }

        private void ConvertDisplacementToLatLon(double displacementXUnits, double displacementYUnits, out double latitude, out double longitude)
        {
            double earthRadius = GetEarthRadius();
            double shipLatRad = _shipLatitude * Math.PI / 180.0;
            double deltaLatRadians = displacementYUnits / earthRadius;
            double deltaLonRadians = displacementXUnits / (earthRadius * Math.Cos(shipLatRad));
            latitude = _shipLatitude + deltaLatRadians * (180.0 / Math.PI);
            longitude = _shipLongitude + deltaLonRadians * (180.0 / Math.PI);
        }
        private void Tb_centerLocDouble_TextChanged(object sender, EventArgs e)
        {
            string text = tb_cneterLocDouble.Text.Trim();
            string[] parts = text.Split(',');
            bool latParsed = false;
            bool lonParsed = false;
            if (parts.Length >= 1)
            {
                string latText = parts[0].Trim();
                if (double.TryParse(latText, NumberStyles.Any, CultureInfo.InvariantCulture, out double latitude))
                {
                    _shipLatitude = latitude;
                    // lbl_shipLatDecimal.Text = latitude.ToString(CultureInfo.InvariantCulture);
                    // lbl_shipLatDecimal.ForeColor = Color.Black;
                    latParsed = true;
                }
                else
                {
                    // lbl_shipLatDecimal.Text = "Invalid Latitude";
                    // lbl_shipLatDecimal.ForeColor = Color.Red;
                }
            }
            else
            {
                // lbl_shipLatDecimal.Text = "Invalid Latitude";
                //  lbl_shipLatDecimal.ForeColor = Color.Red;
            }
            if (parts.Length >= 2)
            {
                string lonText = parts[1].Trim();
                if (double.TryParse(lonText, NumberStyles.Any, CultureInfo.InvariantCulture, out double longitude))
                {
                    _shipLongitude = longitude;
                    //    lbl_shipLonDecimal.Text = longitude.ToString(CultureInfo.InvariantCulture);
                    //    lbl_shipLonDecimal.ForeColor = Color.Black;
                    lonParsed = true;
                }
                else
                {
                    //   lbl_shipLonDecimal.Text = "Invalid Longitude";
                    //   lbl_shipLonDecimal.ForeColor = Color.Red;
                }
            }
            else
            {
                // lbl_shipLonDecimal.Text = "Invalid Longitude";
                //  lbl_shipLonDecimal.ForeColor = Color.Red;
            }
            if (latParsed && lonParsed)
            {
                UpdateArbitraryPoint();
                UpdateDisplacements();
                mapPanel2.Invalidate();
            }
        }

        private void Cb_lockArbit_CheckedChanged(object sender, EventArgs e)
        {
            UpdateArbitraryPoint();
            UpdateDisplacements();
            mapPanel2.Invalidate();
        }
        private void tx_ArbitDotRealCoordinatesTextChanged(object sender, EventArgs e)
        {
            string text = tx_ArbitDotRealCoordinates.Text.Trim();
            string[] parts = text.Split(',');
            bool latParsed = false;
            bool lonParsed = false;
            if (parts.Length >= 1)
            {
                string latText = parts[0].Trim();
                if (double.TryParse(latText, NumberStyles.Any, CultureInfo.InvariantCulture, out double latitude))
                {
                    _arbiLatitude = latitude;
                    latParsed = true;
                }
                else
                {
                }
            }
            if (parts.Length >= 2)
            {
                string lonText = parts[1].Trim();
                if (double.TryParse(lonText, NumberStyles.Any, CultureInfo.InvariantCulture, out double longitude))
                {
                    _arbiLongitude = longitude;
                    lonParsed = true;
                }
                else
                {
                }
            }
            if (latParsed && lonParsed)
            {
                UpdateArbitraryPoint();
                mapPanel2.Invalidate();
            }
            else
            {
            }
        }

    }


}
