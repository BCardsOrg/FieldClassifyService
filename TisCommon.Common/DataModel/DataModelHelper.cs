using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TiS.Core.TisCommon.DataModel
{
	public static class DataModelHelper
	{
		/// <summary>
		/// converts rotation degree to TIS_ROTATION enum value
		/// </summary>
		/// <param name="RotationDegree"></param>
		/// <returns>TIS_ROTATION enum used to store in page data</returns>
		static public TIS_ROTATION ToTIS_ROTATION(int rotationDegree)
		{
			switch (rotationDegree % 360)
			{
				case 0:
					return TIS_ROTATION.ROT_0;
				case 90:
					return TIS_ROTATION.ROT_90;
				case 180:
					return TIS_ROTATION.ROT_180;
				case 270:
					return TIS_ROTATION.ROT_270;
				default:
					return TIS_ROTATION.ROT_0;
			}
		}

		static public RotateFlipType ToRotateFlipType(int rotationDegree)
		{
			switch (rotationDegree % 360)
			{
				case 0:
					return RotateFlipType.RotateNoneFlipNone;
				case 90:
					return RotateFlipType.Rotate90FlipNone;
				case 180:
					return RotateFlipType.Rotate180FlipNone;
				case 270:
					return RotateFlipType.Rotate270FlipNone;
				default:
					return RotateFlipType.RotateNoneFlipNone;
			}
		}

		static public string ToImgEnh(TIS_ROTATION rotation)
		{
			switch (rotation)
			{
				case TIS_ROTATION.ROT_0:
					return string.Empty;
				case TIS_ROTATION.ROT_90:
					return  @"/r1";
				case TIS_ROTATION.ROT_180:
					return @"/r2";
				case TIS_ROTATION.ROT_270:
					return @"/r3";
				default:
					return string.Empty;
			}
		}

		static public int ToAngleDeg(TIS_ROTATION rotation)
		{
			switch (rotation)
			{
				case TIS_ROTATION.ROT_0:
					return 0;
				case TIS_ROTATION.ROT_90:
					return 90;
				case TIS_ROTATION.ROT_180:
					return 180;
				case TIS_ROTATION.ROT_270:
					return 270;
				default:
					return 0;
			}
		}

	}
}
