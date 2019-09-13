using Kurenaiz.Utilities.Types;
using Seega.Enums;

public interface ICaptureVerifier
{
    bool VerifyCapture(Safe2DArray fields, TileField currentField, ColorType allyColor);
}