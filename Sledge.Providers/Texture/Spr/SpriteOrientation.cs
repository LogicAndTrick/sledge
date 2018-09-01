namespace Sledge.Providers.Texture.Spr
{
    // The Quake source code in client/r_sprite.c has more info on this...
    public enum SpriteOrientation
    {
        ParallelUpright = 0, // Face camera, rotates around Z axis
        FacingUpright = 1,   // Do strange, seemingly pointless things (turn away as the camera approaches)
        Parallel = 2,        // Roll angle works, otherwise face the camera
        Oriented = 3,        // Use fixed angles
        ParallelOriented = 4 // Identical to parallel
    }
}