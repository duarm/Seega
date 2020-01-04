using Seega.Types;

public interface ICaptureVerifier
{
    int VerifyCapture(TileField currentField, Movement movement);
}