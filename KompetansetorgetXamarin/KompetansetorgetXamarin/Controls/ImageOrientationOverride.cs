using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//The original Xlabs.enum.ImageOrientation didn't register ImageCentered for some reason so I replaced it with this

namespace KompetansetorgetXamarin.Controls
{
        public enum ImageOrientationOverride
        {
            /// <summary>
            /// The image to left
            /// </summary>
            ImageToLeft = 0,
            /// <summary>
            /// The image on top
            /// </summary>
            ImageOnTop = 1,
            /// <summary>
            /// The image to right
            /// </summary>
            ImageToRight = 2,
            /// <summary>
            /// The image on bottom
            /// </summary>
            ImageOnBottom = 3,
            /// <summary>
            /// The image centered
            /// </summary>
            ImageCentered = 4
        }

}
