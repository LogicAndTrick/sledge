using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.Common.Extensions;
using Sledge.DataStructures.Models;
using Sledge.FileSystem;
using Sledge.Packages.Vpk;
using Sledge.Packages.Wad;
using Sledge.Providers.Model;
using Sledge.Providers.Texture;

namespace Sledge.Tests.Benchmarking
{
    [TestClass]
    public class ModelLoadTest
    {
        private List<long> Benchmark<T>(Func<T> action, Action<T> destructor, int iterations = 10)
        {
            var times = new List<long>();
            var watch = new Stopwatch();

            // Warm up
            for (int i = 0; i < (iterations / 2); i++)
            {
                destructor(action());
            }

            // actual test
            for (var i = 0; i < iterations + 2; i++)
            {
                watch.Start();
                var t = action();
                watch.Stop();

                destructor(t);

                times.Add(watch.ElapsedMilliseconds);
                watch.Reset();
            }


            // remove max and min from results
            times.Remove(times.Max());
            times.Remove(times.Min());

            var sum = times.Sum();
            var avg = sum / times.Count;

            Debug.WriteLine($"{times.Count} iterations took {sum}ms. Average: {avg}ms, Min: {times.Min()}ms, Max: {times.Max()}ms.");

            return times;
        }

        [TestMethod]
        public void TestMdlLoading1()
        {
            var model = @"D:\Github\sledge\_Resources\MDL\HL1_10\barney.mdl";
            var file = new NativeFile(model);

            ModelProvider.Register(new MdlProvider());

            Benchmark(
                () => ModelProvider.CreateModelReference(file),
                ModelProvider.DeleteModelReference, 100);
        }
        
        private void AssertModelsAreIdentical(Model expected, Model actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.BonesTransformMesh, actual.BonesTransformMesh);

            for (int i = 0; i < expected.Bones.Count; i++)
            {
                var bone = expected.Bones[i];
                var bone2 = actual.Bones[i];

                Assert.AreEqual(bone.BoneIndex, bone2.BoneIndex);
                Assert.AreEqual(bone.ParentIndex, bone2.ParentIndex);
                Assert.AreEqual(bone.Name, bone2.Name);
                Assert.AreEqual(bone.DefaultPosition, bone2.DefaultPosition);
                Assert.AreEqual(bone.DefaultAngles, bone2.DefaultAngles);
                Assert.AreEqual(bone.DefaultPositionScale, bone2.DefaultPositionScale);
                Assert.AreEqual(bone.DefaultAnglesScale, bone2.DefaultAnglesScale);
                Assert.AreEqual(bone.Transform, bone2.Transform);
            }

            for (int i = 0; i < expected.BodyParts.Count; i++)
            {
                var bodyPart = expected.BodyParts[i];
                var bodyPart2 = actual.BodyParts[i];

                Assert.AreEqual(bodyPart.Name, bodyPart2.Name);
                Assert.AreEqual(bodyPart.ActiveMesh, bodyPart2.ActiveMesh);

                foreach (var key in bodyPart.Meshes.Keys)
                {
                    var meshes = bodyPart.Meshes[key];
                    var meshes2 = bodyPart2.Meshes[key];

                    for (int j = 0; j < meshes.Count; j++)
                    {
                        var mesh = meshes[j];
                        var mesh2 = meshes2[j];

                        Assert.AreEqual(mesh.LOD, mesh2.LOD);
                        Assert.AreEqual(mesh.SkinRef, mesh2.SkinRef);

                        for (int k = 0; k < mesh.Vertices.Count; k++)
                        {
                            var vertex = mesh.Vertices[k];
                            var vertex2 = mesh2.Vertices[k];

                            Assert.AreEqual(vertex.Location, vertex2.Location);
                            Assert.AreEqual(vertex.Normal, vertex2.Normal);
                            Assert.AreEqual(vertex.TextureU, vertex2.TextureU);
                            Assert.AreEqual(vertex.TextureV, vertex2.TextureV);

                            var boneWeightings = vertex.BoneWeightings.ToList();
                            var boneWeightings2 = vertex2.BoneWeightings.ToList();

                            for (int l = 0; l < boneWeightings.Count; l++)
                            {
                                var bw = boneWeightings[l];
                                var bw2 = boneWeightings2[l];

                                Assert.AreEqual(bw.Bone.Name, bw2.Bone.Name);
                                Assert.AreEqual(bw.Weight, bw2.Weight);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < expected.Animations.Count; i++)
            {
                var animation = expected.Animations[i];
                var animation2 = actual.Animations[i];

                for (int j = 0; j < animation.Frames.Count; j++)
                {
                    var frame = animation.Frames[j];
                    var frame2 = animation2.Frames[j];

                    for (int k = 0; k < frame.Bones.Count; k++)
                    {
                        var bone = frame.Bones[k];
                        var bone2 = frame2.Bones[k];

                        Assert.AreEqual(bone.Bone.Name, bone2.Bone.Name);
                        Assert.AreEqual(bone.Angles, bone2.Angles, $"Animation: {i}; Frame: {j}; Bone: {k}/{bone.Bone.Name}");
                        Assert.AreEqual(bone.Position, bone2.Position);
                    }
                }
            }
        }
    }
}
