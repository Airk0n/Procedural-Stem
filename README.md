# Procedural Stem
To produce a procedural mesh of a stem I decided to generate it in 3 parts - a **spline** that generates the path of the stem, a **mesh** that takes a spline and follows it and then a **scriptable config** which allow for presets to be easily stored and modified. The thickness tapers towards the top of the mesh whilst following the base thickness and top thickness given in the `SplinePreset`.

![image](https://github.com/user-attachments/assets/ea3d28d8-03cd-4a74-9f87-50bfa570345c)

## StemPreset

The scriptable object `StemPreset` is where we manipulate the mesh with the properties provided:
![image](https://github.com/user-attachments/assets/a4ad9c63-8913-459b-888f-34b95741d20e)

These variables are then mapped to some min and max ranges in both the `StemSpline` and the `StemMesh` scripts:

![image](https://github.com/user-attachments/assets/1a3c440c-f925-42d6-9d8e-c913af4f8b01)
![image](https://github.com/user-attachments/assets/bb75b5af-e0d9-473d-b7f6-0f23341d2480)

## Roughness
Roughness controls 2 main parameters both of which are on the `StemSpline`:
 - How many nodes and control nodes are used on the spline (Lerping between 2 values set in the spline script)
 - The variability on the x and z axis (I called it Lateral Spread), the higher this value the less straight the spline.

**Out of scope:**
A circle is used as the basis of the mesh, if I was to extend this I could look into dividing the base into multiple circles, creating texture along the length of the stem which could scale with roughness.
This could also be done with a displacement texture and/or a normal map.
![image](https://github.com/user-attachments/assets/0373c091-5a7c-418c-9b0e-c628c57c6774)

## Limitations
Joint orientation:
In following the tangent of the spline I struggled to get the end tangent of one segment to line up with the tangent of the next segment. the result is quite extreme twisting at the intersection between segments:

![image](https://github.com/user-attachments/assets/8de50b94-cc72-477a-8d9b-db73a6eb0ef3)

Similarly by following the spline I essentially have 2 circles, one starting at one spline and ending at another, the result is some clipping between the 2 segments:

![image](https://github.com/user-attachments/assets/ee625708-a76a-4015-8354-3cda3d327b04)

The first node in the spline is not oriented to be flat, this could be easily fixed by removing lateral randomness from the first couple of nodes in the spline.

![image](https://github.com/user-attachments/assets/c0d7a76f-7032-45ff-95c3-5e8a1cdfb162)




