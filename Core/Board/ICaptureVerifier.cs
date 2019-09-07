using Kurenaiz.Utilities.Types;
using Seega.GlobalEnums;

namespace Seega.Scripts.Core
{
    public interface ICaptureVerifier
    {
        void VerifyCapture(TileField currentField, PieceType allyColor, Safe2DArray fields);
    }
}