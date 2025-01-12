#ifndef GRAPHIC_HELPER_INCLUDE
#define GRAPHIC_HELPER_INCLUDE

#ifndef UNITY_PI
#define UNITY_PI 3.1415926535
#endif
// github.com/GarrettGunnell/Grass/blob/main/Assets/Shaders/ModelGrass.shader
float4 RotateAroundYInDegrees(float4 vertex, float degrees)
{
    float alpha = degrees * UNITY_PI / 180.0;
    float sina, cosa;
    sincos(alpha, sina, cosa);
    float2x2 m = float2x2(cosa, -sina, sina, cosa);
    return float4(mul(m, vertex.xz), vertex.yw).xzyw;
}
float4 RotateAroundXInDegrees(float4 vertex, float degrees)
{
    float alpha = degrees * UNITY_PI / 180.0;
    float sina, cosa;
    sincos(alpha, sina, cosa);
    float2x2 m = float2x2(cosa, -sina, sina, cosa);
    return float4(mul(m, vertex.yz), vertex.xw).zxyw;
}

float4 RotateAroundAxis(float4 vertex, float3 axis, float angle)
{
    float radians = angle * UNITY_PI / 180.0;
    float sina, cosa;
    sincos(radians, sina, cosa);

    // Rodrigues' rotation formula
    float3 rotatedVertex = vertex.xyz * cosa +
                           cross(axis, vertex.xyz) * sina +
                           axis * dot(axis, vertex.xyz) * (1 - cosa);

    return float4(rotatedVertex, vertex.w);
}
half4 RotateAroundAxis_half(half4 vertex, half3 axis, half angle)
{
    half radians = angle * UNITY_PI / 180.0;
    half sina, cosa;
    sincos(radians, sina, cosa);

    // Rodrigues' rotation formula
    half3 rotatedVertex = vertex.xyz * cosa +
                           cross(axis, vertex.xyz) * sina +
                           axis * dot(axis, vertex.xyz) * (1 - cosa);

    return half4(rotatedVertex, vertex.w);
}
float4 RotateAroundAxis(float4 vertex, float3 axis, float angle, float3 center)
{

    float3 translatedVertex = vertex.xyz - center;
    float radians = angle * UNITY_PI / 180.0;
    float sina, cosa;
    sincos(radians, sina, cosa);
    
    float3 rotatedTranslatedVertex = translatedVertex * cosa +
                                     cross(axis, translatedVertex) * sina +
                                     axis * dot(axis, translatedVertex) * (1 - cosa);

    float3 rotatedVertex = rotatedTranslatedVertex + center;
    return float4(rotatedVertex, vertex.w);
}
half4 RotateAroundAxis_half(half4 vertex, half3 axis, half angle, half3 center)
{

    half3 translatedVertex = vertex.xyz - center;
    half radians = angle * UNITY_PI / 180.0;
    half sina, cosa;
    sincos(radians, sina, cosa);
    
    half3 rotatedTranslatedVertex = translatedVertex * cosa +
                                     cross(axis, translatedVertex) * sina +
                                     axis * dot(axis, translatedVertex) * (1 - cosa);

    half3 rotatedVertex = rotatedTranslatedVertex + center;
    return half4(rotatedVertex, vertex.w);
}

float2 Rotate2D(float2 uv, float angle)
{
    float alpha = angle * UNITY_PI / 180.0;
    float sina, cosa;
    sincos(alpha, sina, cosa);
    float2x2 m = float2x2(cosa, -sina, sina, cosa);
    return mul(m, uv);

}

void CubicBezierCurve(float3 P0, float3 P1, float3 P2, float3 P3, float t, out float3 pos, out float3 tangent)
{
    float t2 = t * t;
    float t3 = t * t * t;
    float4x3 input =
    {
        P0.x, P0.y, P0.z,
        P1.x, P1.y, P1.z,
        P2.x, P2.y, P2.z,
        P3.x, P3.y, P3.z
    };

    float1x4 bernstein =
    {
         1 - 3 * t + 3 * t2 - 3 * t3,
         3 * t - 6 * t2 + 3 * t3,
         3 * t2 - 3 * t3,
         t3
    };

    float1x4 d_bernstein =
    {
        -3 + 6 * t - 9 * t2,
        3 - 12 * t + 9 * t2,
        6 * t - 9 * t2,
        3 * t2
    };
    pos = mul(bernstein, input);
    tangent = mul(d_bernstein, input);
}
void CubicBezierCurve_Tilt_Bend(float3 P2, float3 P3, float t, out float3 pos, out float3 tangent)
{
    float t2 = t * t;
    float t3 = t * t * t;
    float1x2 bernstein =
    {
         3 * t2 - 3 * t3,
         t3
    };

    float2x3 input =
    {
        P2.x, P2.y, P2.z,
        P3.x, P3.y, P3.z
    };


    float1x2 d_bernstein =
    {
        6 * t - 9 * t2,
        3 * t2
    };
    pos = mul(bernstein, input);
    tangent = mul(d_bernstein, input);
}

void CubicBezierCurve_Tilt_Bend_half(half3 P2, half3 P3, half t, out half3 pos, out half3 tangent)
{
    half t2 = t * t;
    half t3 = t * t * t;
    half1x2 bernstein =
    {
        3 * t2 - 3 * t3,
         t3
    };

    half2x3 input =
    {
        P2.x, P2.y, P2.z,
        P3.x, P3.y, P3.z
    };


    half1x2 d_bernstein =
    {
        6 * t - 9 * t2,
        3 * t2
    };
    pos = mul(bernstein, input);
    tangent = mul(d_bernstein, input);
}

float3 ScaleWithCenter(float3 pos,float scale, float3 center)
{
    pos -= center;
    pos *= scale;
    pos += center;
    return pos;
}
float3 ScaleWithCenter(float3 pos, float3 scale, float3 center)
{
    pos -= center;
    pos.x *= scale.x;
    pos.y *= scale.y;
    pos.z *= scale.z;
    pos += center;
    return pos;
}
float3 ProjectOntoPlane(float3 v, float3 planeNormal)
{
    return v - dot(v, normalize(planeNormal)) * planeNormal;
}
//gist.github.com/outsidecontext/6083f490d4bd56b3e34b7893e6a34480
float4 slerp(float4 v0, float4 v1, float t)
{
    
    // Compute the cosine of the angle between the two vectors.
    float d = dot(v0, v1);

    const float DOT_THRESHOLD = 0.9995;
    if (abs(d) > DOT_THRESHOLD)
    {
        // If the inputs are too close for comfort, linearly interpolate
        // and normalize the result.
        float4 result = v0 + t * (v1 - v0);
        normalize(result);
        return result;
    }

    // If the dot product is negative, the quaternions
    // have opposite handed-ness and slerp won't take
    // the shorter path. Fix by reversing one quaternion.
    if (d < 0.0f)
    {
        v1 = -v1;
        d = -d;
    }

    clamp(d, -1, 1); // Robustness: Stay within domain of acos()
    float theta_0 = acos(d); // theta_0 = angle between input vectors
    float theta = theta_0 * t; // theta = angle between v0 and result 

    float4 v2 = v1 - v0 * d;
    normalize(v2); // { v0, v2 } is now an orthonormal basis

    return v0 * cos(theta) + v2 * sin(theta);
}
// Function to compute the quaternion rotation
float4 ComputeQuaternionRotation(float3 from, float3 to)
{
    float3 halfRot = normalize (from + to);
    float4 quat;
    quat.xyz = cross(from, halfRot);
    quat.w = dot(from, halfRot);
    return quat;
}



// Function to apply a quaternion to a vector
float4 ApplyQuaternion(float4 q, float3 v)
{
    float3 qvec = q.xyz;
    float3 uv = cross(qvec, v);
    float3 uuv = cross(qvec, uv);
    uv *= (2.0 * q.w);
    uuv *= 2.0;
    return float4(v + uv + uuv, 1.0);
}

float4 TransformWithAlignment(float4 input, float3 align, float3 target)
{
    float4 rotationQuat = ComputeQuaternionRotation(align, target);
    float3 transformed = ApplyQuaternion(rotationQuat, input.xyz).xyz;
    return float4(transformed, input.w);
}
float4 TransformWithAlignment(float4 input, float3 align, float3 target, float3 pivot)
{
    float3 translatedInput = input.xyz - pivot;
    float4 rotationQuat = ComputeQuaternionRotation(align, target);
    float3 rotated = ApplyQuaternion(rotationQuat, translatedInput).xyz;
    float3 finalPosition = rotated + pivot;
    return float4(finalPosition, input.w);
}

float2 ReverseAtan2(float theta)
{
    float x = cos(theta);
    float y = sin(theta);
    return float2(x, y);
}
float2 ReverseAtan2Degrees(float thetaDegrees)
{
    float thetaRadians = thetaDegrees * (UNITY_PI / 180.0);
    
    float x = cos(thetaRadians);
    float y = sin(thetaRadians);
    
    return float2(x, y);
}

half2 ReverseAtan2Degrees_half(half thetaDegrees)
{
    half thetaRadians = thetaDegrees * (UNITY_PI / 180.0);
    
    half x = cos(thetaRadians);
    half y = sin(thetaRadians);
    
    return half2(x, y);
}

bool IsPointInTriangle(float2 tri[3], float2 p)
{
    float2 v0 = tri[1] - tri[0];
    float2 v1 = tri[2] - tri[0];
    float2 v2 = p - tri[0];

    float d00 = dot(v0, v0);
    float d01 = dot(v0, v1);
    float d11 = dot(v1, v1);
    float d20 = dot(v2, v0);
    float d21 = dot(v2, v1);

    float denom = d00 * d11 - d01 * d01;
    
    if (denom == 0)
        return false;
    
    float v = (d11 * d20 - d01 * d21) / denom;
    float w = (d00 * d21 - d01 * d20) / denom;
    float u = 1.0 - v - w;

    return (u >= 0 && v >= 0 && w >= 0);
}
// Ref: http://jcgt.org/published/0003/02/01/paper.pdf "A Survey of Efficient Representations for Independent Unit Vectors"
// Encode with Oct, this function work with any size of output
// return float between [-1, 1]
float2 PackNormalOctQuadEncode_copy(float3 n)
{
    //float l1norm    = dot(abs(n), 1.0);
    //float2 res0     = n.xy * (1.0 / l1norm);

    //float2 val      = 1.0 - abs(res0.yx);
    //return (n.zz < float2(0.0, 0.0) ? (res0 >= 0.0 ? val : -val) : res0);

    // Optimized version of above code:
    n *= rcp(max(dot(abs(n), 1.0), 1e-6));
    float t = saturate(-n.z);
    return n.xy + float2(n.x >= 0.0 ? t : -t, n.y >= 0.0 ? t : -t);
}

float3 UnpackNormalOctQuadEncode_copy(float2 f)
{
    float3 n = float3(f.x, f.y, 1.0 - abs(f.x) - abs(f.y));

    //float2 val = 1.0 - abs(n.yx);
    //n.xy = (n.zz < float2(0.0, 0.0) ? (n.xy >= 0.0 ? val : -val) : n.xy);

    // Optimized version of above code:
    float t = max(-n.z, 0.0);
    n.xy += float2(n.x >= 0.0 ? -t : t, n.y >= 0.0 ? -t : t);

    return normalize(n);
}
#endif