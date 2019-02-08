namespace CoordinatorMap
{
    partial class CoordinatorMap
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this._map = new GMap.NET.WindowsForms.GMapControl();
            this.SuspendLayout();
            // 
            // _map
            // 
            this._map.Bearing = 0F;
            this._map.CanDragMap = true;
            this._map.EmptyTileColor = System.Drawing.Color.Navy;
            this._map.GrayScaleMode = false;
            this._map.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this._map.LevelsKeepInMemmory = 5;
            this._map.Location = new System.Drawing.Point(0, 0);
            this._map.MarkersEnabled = true;
            this._map.MaxZoom = 21;
            this._map.MinZoom = 17;
            this._map.MouseWheelZoomEnabled = true;
            this._map.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this._map.Name = "_map";
            this._map.NegativeMode = false;
            this._map.PolygonsEnabled = true;
            this._map.RetryLoadTile = 0;
            this._map.RoutesEnabled = true;
            this._map.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this._map.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this._map.ShowTileGridLines = false;
            this._map.Size = this.ClientSize;
            this._map.TabIndex = 0;
            this._map.Zoom = 19;
            this._map.Load += Map_Load;
            this._map.MouseMove += Map_MouseMove;
            this._map.MouseLeave += Map_MouseLeave;
            // 
            // CoordinatorMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 350);
            this.Controls.Add(this._map);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CoordinatorMap";
            this.ResumeLayout(false);
        }

        #endregion
        
        public new System.Drawing.Size ClientSize
        {
            get
            {
                return base.ClientSize;
            }

            set
            {
                base.ClientSize = value;
                Map.ClientSize = value;
                if (MapHasLoaded) CorrectPosition();
            }
        }

        private GMap.NET.WindowsForms.GMapControl _map;
        public GMap.NET.WindowsForms.GMapControl Map { get => _map; }
    }
}