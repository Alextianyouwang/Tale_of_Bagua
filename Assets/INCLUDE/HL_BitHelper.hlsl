#ifndef BIT_HELPER_INCLUDED
#define BIT_HELPER_INCLUDED
float Pack2FloatsTo32BitNormal(float x, float y)
{
    // Convert from [-1, 1] to [0, 1] range for packing
    x = (x + 1.0f) * 0.5f;
    y = (y + 1.0f) * 0.5f;

    // Scale to 16-bit fixed-point (0 to 65535)
    uint xInt = (uint) (x * 65535.0f);
    uint yInt = (uint) (y * 65535.0f);

    // Pack the two 16-bit values into a 32-bit uint
    uint packed = (xInt << 16) | yInt;

    // Reinterpret as a float
    return asfloat(packed);
}

void Unpack32BitTo2FloatsNormal(float packedFloat, out float x, out float y)
{
    // Reinterpret the float as a 32-bit uint
    uint packed = asuint(packedFloat);

    // Extract the 16-bit values
    uint xInt = (packed >> 16) & 0xFFFF;
    uint yInt = packed & 0xFFFF;

    // Convert back to [0, 1] range
    x = xInt / 65535.0f;
    y = yInt / 65535.0f;

    // Convert from [0, 1] back to [-1, 1] range
    x = x * 2.0f - 1.0f;
    y = y * 2.0f - 1.0f;
}
float Pack2FloatsTo32Bit(float x, float y)
{
    // Ensure the inputs are clamped to [0, 1]
    x = saturate(x);
    y = saturate(y);

    // Convert to 16-bit fixed-point (0 to 65535)
    uint xInt = (uint)(x * 65535.0f);
    uint yInt = (uint)(y * 65535.0f);

    // Pack the two 16-bit values into a single 32-bit uint
    uint packed = (xInt << 16) | yInt;

    // Reinterpret as a float for storage
    return asfloat(packed);
}

void Unpack32BitTo2Floats(float packedFloat, out float x, out float y)
{
    // Reinterpret the float as a 32-bit unsigned integer
    uint packed = asuint(packedFloat);

    // Extract the 16-bit values
    uint xInt = (packed >> 16) & 0xFFFF;
    uint yInt = packed & 0xFFFF;

    // Convert back to the [0, 1] range
    x = xInt / 65535.0f;
    y = yInt / 65535.0f;
}
float PackFloats24_8(float a, float b)
{
    // Convert 'a' to 24-bit precision (truncate 8 least significant bits)
    uint a32 = asuint(a); // Reinterpret 'a' as a uint (32 bits)
    uint a24 = (a32 >> 8) & 0xFFFFFF; // Keep only the upper 24 bits

    // Convert 'b' to 8-bit value (quantize to 0-255)
    uint b8 = (uint) clamp(b * 255.0f, 0.0f, 255.0f); // Scale to 0-255 and clamp

    // Pack a24 and b8 into one 32-bit value
    uint packed = (a24 << 8) | b8;

    // Reinterpret as float
    return asfloat(packed);
}
void UnpackFloats24_8(float input, out float a, out float b)
{
    // Reinterpret the input float as uint
    uint packed = asuint(input);

    // Extract the 24-bit value for 'a'
    uint a24 = (packed >> 8) & 0xFFFFFF;
    uint a32 = a24 << 8; // Shift back to the original 32-bit alignment

    // Extract the 8-bit value for 'b'
    uint b8 = packed & 0xFF;

    // Convert back to float values
    a = asfloat(a32); // Reinterpret as float
    b = (float) b8 / 255.0f; // Convert back from 0-255 to [0, 1] range
}
float PackFloats22_8_2(float a, float b, uint flags)
{
    // Ensure the flags are within 2 bits (0 to 3)
    flags = flags & 0x3; // Mask out only the lowest 2 bits

    // Convert 'a' to 22 bits (truncate lower 10 bits)
    uint a32 = asuint(a); // Reinterpret 'a' as uint (32 bits)
    uint a22 = (a32 >> 10) & 0x3FFFFF; // Keep only the upper 22 bits

    // Convert 'b' to 8-bit integer value (0-255)
    uint b8 = (uint) clamp(b * 255.0f, 0.0f, 255.0f); // Scale and clamp

    // Pack the values into a single 32-bit integer
    uint packed = (a22 << 10) | (b8 << 2) | flags;

    // Reinterpret as float for storage
    return asfloat(packed);
}
void UnpackFloats22_8_2(float input, out float a, out float b, out uint flags)
{
    // Reinterpret the input float as uint
    uint packed = asuint(input);

    // Extract the 2-bit flags
    flags = packed & 0x3; // Mask the lowest 2 bits

    // Extract the 8-bit value for 'b'
    uint b8 = (packed >> 2) & 0xFF; // Shift and mask 8 bits
    b = (float) b8 / 255.0f; // Convert back to [0, 1] range

    // Extract the 22-bit value for 'a'
    uint a22 = (packed >> 10) & 0x3FFFFF; // Shift and mask 22 bits
    uint a32 = a22 << 10; // Shift back to align with original 32-bit float layout

    // Convert back to float
    a = asfloat(a32);
}
float PackFloats24_6_2(float a, float b, uint flags)
{
    // Ensure flags are within 2 bits (0-3).
    flags = flags & 0x3;

    // Convert 'a' to 24 bits (discard the lowest 8 bits).
    uint a32 = asuint(a); // Reinterpret as uint.
    uint a24 = (a32 >> 8) & 0xFFFFFF; // Keep the upper 24 bits.

    // Convert 'b' to 6 bits (scale to 0-63).
    uint b6 = (uint) clamp(b * 63.0f, 0.0f, 63.0f);

    // Pack values: a24 (24 bits), b6 (6 bits), flags (2 bits).
    uint packed = (a24 << 8) | (b6 << 2) | flags;

    // Reinterpret the packed value as a float.
    return asfloat(packed);
}
void UnpackFloats24_6_2(float input, out float a, out float b, out uint flags)
{
    // Reinterpret input as a uint.
    uint packed = asuint(input);

    // Extract the 2-bit flags.
    flags = packed & 0x3;

    // Extract the 6-bit value for 'b'.
    uint b6 = (packed >> 2) & 0x3F; // Shift and mask 6 bits.
    b = (float) b6 / 63.0f; // Scale back to [0, 1] range.

    // Extract the 24-bit value for 'a'.
    uint a24 = (packed >> 8) & 0xFFFFFF; // Shift and mask 24 bits.
    uint a32 = a24 << 8; // Restore alignment with original float layout.

    // Convert back to float.
    a = asfloat(a32);
}
float Pack4FloatsTo32Bit(float r, float g, float b, float a)
{
    // Ensure the input floats are clamped to the range [0, 1]
    r = saturate(r);
    g = saturate(g);
    b = saturate(b);
    a = saturate(a);

    // Convert to 8-bit fixed-point values (0-255)
    uint rInt = (uint) (r * 255.0f);
    uint gInt = (uint) (g * 255.0f);
    uint bInt = (uint) (b * 255.0f);
    uint aInt = (uint) (a * 255.0f);

    // Pack the 4x8-bit values into a single uint
    uint packed = (rInt << 24) | (gInt << 16) | (bInt << 8) | aInt;

    // Reinterpret the packed uint as a float
    return asfloat(packed);
}
void Unpack32BitTo4Floats(float packedFloat, out float r, out float g, out float b, out float a)
{
    // Reinterpret the float as a 32-bit unsigned integer
    uint packed = asuint(packedFloat);

    // Extract the 8-bit values and convert them back to floats
    r = ((packed >> 24) & 0xFF) / 255.0f;
    g = ((packed >> 16) & 0xFF) / 255.0f;
    b = ((packed >> 8) & 0xFF) / 255.0f;
    a = (packed & 0xFF) / 255.0f;
}
#endif