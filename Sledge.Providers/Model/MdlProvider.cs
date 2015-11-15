using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Models;
using Sledge.FileSystem;

namespace Sledge.Providers.Model
{
    [Flags]
    public enum ModelLoadItems
    {
        Bones = 1 << 0,
        Meshes = 1 << 1,
        Animations = 1 << 2,
        TextureInfo = 1 << 3,
        TextureData = 1 << 4,

        Textures = TextureInfo | TextureData,
        AllStaticNoTextures = Bones | Meshes,
        AllStatic = Bones | Meshes | Textures,
        All = Bones | Meshes | Animations | Textures
    }

    /// <summary>
    /// Loader for Source and GoldSource MDL files.
    /// // TODO: Source animation support! Currently reads animations fine, but displays them incorrectly!
    /// </summary>
    public class MdlProvider : ModelProvider
    {
        protected override bool IsValidForFile(IFile file)
        {
            return file.Extension.ToLowerInvariant() == "mdl";
        }

        protected override DataStructures.Models.Model LoadFromFile(IFile file, DataStructures.GameData.Palette pal)
        {
            return LoadMDL(file, ModelLoadItems.AllStatic | ModelLoadItems.Animations, pal);
        }

        // Model loader for MDL files. Reference Valve's studiohdr_t struct definition for the most part.
        public DataStructures.Models.Model LoadMDL(IFile file, ModelLoadItems loadItems, DataStructures.GameData.Palette pal)
        {
            using (var fs = new MemoryStream(file.ReadAll()))
            {
                using(var br = new BinaryReader(fs))
                {
                    return ReadModel(br, file, loadItems, pal);
                }
            }
        }

        private const string MagicStringIDST = "IDST";
        private const string MagicStringIDSQ = "IDSQ";
        private const string MagicStringIDSV = "IDSV";

        private const int MDLVersionGoldsource = 10; // All GS games
        private const int MDLVersionSource2006 = 44; // HL2, CSS, EP1, LC
        private const int MDLVersionSourceEpisode2 = 45; // EP2
        private const int MDLVersionSourcePortal = 46; // Portal
        private const int MDLVersionSource2007 = 48; // TF2
        private const int MDLVersionSource2012 = 49; // AS, CSGO, DOTA2, L4D, L4D2, Portal 2
        
        private const int VVDVersionSource = 4;
        private const int VTXVersionSource = 7;

        private const byte VTXStripGroupTriListFlag = 0x01;
        private const byte VTXStripGroupTriStripFlag = 0x02;

        private static DataStructures.Models.Model ReadModel(BinaryReader br, IFile file, ModelLoadItems loadItems, DataStructures.GameData.Palette pal)
        {
            // int id - Not really an int. This is a magic string, either "IDST" or "IDSQ".
            var magicString = br.ReadFixedLengthString(Encoding.UTF8, 4);
            if (magicString != MagicStringIDST && magicString != MagicStringIDSQ)
            {
                throw new ProviderException("Bad magic number for model. Expected [IDST,IDSQ], got: " + magicString);
            }

            // int version - Half-life 1 models are version 10.
            var version = br.ReadInt32();
            if (version != MDLVersionGoldsource
                && version != MDLVersionSource2006
                && version != MDLVersionSourceEpisode2
                && version != MDLVersionSourcePortal
                && version != MDLVersionSource2007
                && version != MDLVersionSource2012)
            {
                throw new ProviderException("Bad version number for model. Expected [10,44,45,46,48,49], got: " + version);
            }

            var modelData = new ModelData {Version = version};

            if (version >= MDLVersionSource2006)
            {
                if (loadItems.HasFlag(ModelLoadItems.Meshes))
                {
                    // Source vertex and mesh info is stored in flat file structures, preload these separately.
                    LoadSourceMeshData(modelData, file);
                }
            }

            long checksum = 0;
            if (version >= MDLVersionSource2006)
            {
                checksum = br.ReadInt32(); // This is a long in the headers but is only written to the file in 4 bytes. Why? I don't know.
            }

            // char name[64] - The name of the model (file path)
            var path = br.ReadFixedLengthString(Encoding.UTF8, 64);

            // int length - The size of the model file in bytes
            var fileSize = br.ReadInt32();

            var eyePosition = br.ReadCoordinateF();

            var illumPosition = CoordinateF.Zero;
            if (version >= MDLVersionSource2006)
            {
                illumPosition = br.ReadCoordinateF();
            }

            var hullMin = br.ReadCoordinateF();
            var hullMax = br.ReadCoordinateF();

            var bbMin = br.ReadCoordinateF();
            var bbMax = br.ReadCoordinateF();

            // int flags - Unknown.
            var flags = br.ReadInt32();

            var numBones = br.ReadInt32();
            var boneIndex = br.ReadInt32();

            var numBoneControllers = br.ReadInt32();
            var boneControllerIndex = br.ReadInt32();

            var numHitBoxes = br.ReadInt32();
            var hitboxIndex = br.ReadInt32();

            if (version >= MDLVersionSource2006)
            {
                var numAnim = br.ReadInt32();
                var animIndex = br.ReadInt32();
                if (loadItems.HasFlag(ModelLoadItems.Animations))
                {
                    // Source animation data is stored on their own instead of inside the sequence
                    LoadSourceAnimationData(br, modelData, numAnim, animIndex);
                }
            }

            var numSeq = br.ReadInt32();
            var seqIndex = br.ReadInt32();

            int numSeqGroups = 0, seqGroupIndex = 0, activitylistversion = 0, eventsindexed = 0;
            if (version >= MDLVersionSource2006)
            {
                activitylistversion = br.ReadInt32();
                eventsindexed = br.ReadInt32();
            }
            else if (version == MDLVersionGoldsource)
            {
                numSeqGroups = br.ReadInt32();
                seqGroupIndex = br.ReadInt32();
            }

            var numTextures = br.ReadInt32();
            var textureIndex = br.ReadInt32();
            var textureDataIndex = 0;

            if (version == MDLVersionGoldsource)
            {
                textureDataIndex = br.ReadInt32();
            }

            if (version >= MDLVersionSource2006)
            {
                var numcdtextures = br.ReadInt32();
                var cdtextureindex = br.ReadInt32();
            }

            var numSkinRef = br.ReadInt32();
            var numSkinFamilies = br.ReadInt32();
            var skinIndex = br.ReadInt32();

            var numBodyParts = br.ReadInt32();
            var bodyPartIndex = br.ReadInt32();

            var numAttachments = br.ReadInt32();
            var attachmentIndex = br.ReadInt32();

            if (version >= MDLVersionSource2006)
            {
                var numlocalnodes = br.ReadInt32();
                var localnodeindex = br.ReadInt32();
                var localnodenameindex = br.ReadInt32();

                var numflexdesc = br.ReadInt32();
                var flexdescindex = br.ReadInt32();

                var numflexcontrollers = br.ReadInt32();
                var flexcontrollerindex = br.ReadInt32();

                var numflexrules = br.ReadInt32();
                var flexruleindex = br.ReadInt32();

                var numikchains = br.ReadInt32();
                var ikchainindex = br.ReadInt32();

                var nummouths = br.ReadInt32();
                var mouthindex = br.ReadInt32();

                var numlocalposeparameters = br.ReadInt32();
                var localposeparamindex = br.ReadInt32();

                var surfacepropindex = br.ReadInt32();

                var keyvalueindex = br.ReadInt32();
                var keyvaluesize = br.ReadInt32();

                var numlocalikautoplaylocks = br.ReadInt32();
                var localikautoplaylockindex = br.ReadInt32();

                var mass = br.ReadSingle();
                var contents = br.ReadInt32();

                var numincludemodels = br.ReadInt32();
                var includemodelindex = br.ReadInt32();

                var virtualModelPointer = br.ReadInt32();

                var szanimblocknameindex = br.ReadInt32();
                var numanimblocks = br.ReadInt32();
                var animblockindex = br.ReadInt32();
                var animblockModelPointer = br.ReadInt32();

                var bonetablebynameindex = br.ReadInt32();

                var pVertexBasePointer = br.ReadInt32();
                var pIndexBasePointer = br.ReadInt32();

                var constdirectionallightdot = br.ReadByte();
                var rootLod = br.ReadByte();
                var numAllowedRootLods = br.ReadByte(); // Unused in Source2006
                br.ReadByte(); // Unused

                var zeroframecacheindex = br.ReadInt32(); // Unused in Source2007

                if (version == MDLVersionSource2006)
                {
                    br.ReadBytes(6); // Unused
                }
                else if (version == MDLVersionSource2007)
                {
                    var numflexcontrollerui = br.ReadInt32();
                    var flexcontrolleruiindex = br.ReadInt32();

                    br.ReadIntArray(2); // Unused

                    var studiohdr2Index = br.ReadInt32();
                    br.ReadInt32(); // Unused
                }
            }
            else if (version == MDLVersionGoldsource)
            {
                var soundTable = br.ReadInt32();
                var soundIndex = br.ReadInt32();
                var soundGroups = br.ReadInt32();
                var soundGroupIndex = br.ReadInt32();

                var numTransitions = br.ReadInt32();
                var transitionIndex = br.ReadInt32();
            }

            var model = new DataStructures.Models.Model();
            model.Name = file.NameWithoutExtension;
            model.BonesTransformMesh = modelData.Version == MDLVersionGoldsource;

            if (loadItems.HasFlag(ModelLoadItems.Bones))
            {
                // Bones
                br.BaseStream.Position = boneIndex;
                for (var i = 0; i < numBones; i++) ReadBone(br, i, modelData, model);
            }

            // Controllers
            // TODO

            // Attachments
            // TODO

            // Hitboxes
            // TODO

            if (loadItems.HasFlag(ModelLoadItems.Animations))
            {
                if (version >= MDLVersionSource2006)
                {
                    throw new ProviderException("Source animations are currently not supported.");
                }

                // Sequence Groups
                var groups = new List<SequenceGroup>();
                br.BaseStream.Position = seqGroupIndex;
                for (var i = 0; i < numSeqGroups; i++) groups.Add(ReadSequenceGroup(br, modelData));

                // Sequences
                br.BaseStream.Position = seqIndex;
                for (var i = 0; i < numSeq; i++) ReadSequence(br, i, modelData, model, groups);
            }

            // Transitions
            // TODO

            if (loadItems.HasFlag(ModelLoadItems.Meshes))
            {
                // Body parts
                br.BaseStream.Position = bodyPartIndex;
                for (var i = 0; i < numBodyParts; i++) ReadBodyPart(br, i, modelData, model);
            }

            // Texture Info
            if (loadItems.HasFlag(ModelLoadItems.TextureInfo) || loadItems.HasFlag(ModelLoadItems.TextureData))
            {
                ReadTextureInfo(file, br, modelData, model, numTextures, textureIndex);
            }

            // Textures

            return model;
        }

        #region Textures
        private static void ReadTextureInfo(IFile file, BinaryReader br, ModelData data, DataStructures.Models.Model model, int numTextures, int textureIndex)
        {
            if (data.Version == MDLVersionGoldsource)
            {
                var tempBr = br;
                var disp = false;
                if (numTextures == 0)
                {
                    disp = true;
                    var texFile = file.Parent.GetFile(file.NameWithoutExtension + "T." + file.Extension);
                    br = new BinaryReader(texFile.Open());
                    br.BaseStream.Position = 180; // skip all the unused nonsense in the T file
                    numTextures = br.ReadInt32();
                    textureIndex = br.ReadInt32();
                    var textureDataIndex = br.ReadInt32();
                    var numSkinRef = br.ReadInt32();
                    var numSkinFamilies = br.ReadInt32();
                    var skinIndex = br.ReadInt32();
                }
                br.BaseStream.Position = textureIndex;
                for (var i = 0; i < numTextures; i++)
                {
                    var name = br.ReadFixedLengthString(Encoding.ASCII, 64);
                    var flags = br.ReadInt32();
                    var width = br.ReadInt32();
                    var height = br.ReadInt32();
                    var index = br.ReadInt32();

                    var savedPosition = br.BaseStream.Position;
                    br.BaseStream.Position = index;

                    var indices = br.ReadBytes(width * height);
                    var palette = br.ReadBytes((byte.MaxValue + 1) * 3);
                    var bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                    var pal = bmp.Palette;
                    for (var j = 0; j <= byte.MaxValue; j++)
                    {
                        var k = j * 3;
                        pal.Entries[j] = Color.FromArgb(255, palette[k], palette[k + 1], palette[k + 2]);
                    }
                    bmp.Palette = pal;
                    var bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                    Marshal.Copy(indices, 0, bmpData.Scan0, indices.Length);
                    bmp.UnlockBits(bmpData);

                    var tex = new DataStructures.Models.Texture
                                  {
                                      Name = name,
                                      Index = i,
                                      Width = width,
                                      Height = height,
                                      Flags = flags,
                                      Image = bmp
                                  };
                    model.Textures.Add(tex);

                    br.BaseStream.Position = savedPosition;
                }
                //
                if (disp)
                {
                    br.BaseStream.Dispose();
                    br.Dispose();
                    br = tempBr;
                }
            }
            else if (data.Version >= MDLVersionSource2006)
            {
                
            }
        }
        #endregion

        #region Animation/Sequences
        private static void ReadSequence(BinaryReader br, int index, ModelData data, DataStructures.Models.Model model, IList<SequenceGroup> groups)
        {
            var startReadIndex = br.BaseStream.Position;
            var name = "";
            var fps = 0f;
            if (data.Version == MDLVersionGoldsource)
            {
                name = br.ReadFixedLengthString(Encoding.ASCII, 32);
                fps = br.ReadSingle();
            }
            else if (data.Version >= MDLVersionSource2006)
            {
                var baseIndex = br.ReadInt32();
                var labelIndex = br.ReadInt32();
                var activityNameIndex = br.ReadInt32();
            }

            var flags = br.ReadInt32();

            var activity = br.ReadInt32();
            var actweight = br.ReadInt32();

            var numevents = br.ReadInt32();
            var eventindex = br.ReadInt32();

            var numframes = 0;

            if (data.Version == MDLVersionGoldsource)
            {
                numframes = br.ReadInt32();
                var numpivots = br.ReadInt32();
                var pivotindex = br.ReadInt32();
                var motiontype = br.ReadInt32();
                var motionbone = br.ReadInt32();
                var linearmovement = br.ReadCoordinateF();
                var automoveposindex = br.ReadInt32();
                var automoveangleindex = br.ReadInt32();
            }

            var bbmin = br.ReadCoordinateF();
            var bbmax = br.ReadCoordinateF();

            var numblends = br.ReadInt32();

            var animindex = br.ReadInt32();

            var groupsize = new int[0];
            if (data.Version >= MDLVersionSource2006)
            {
                var movementindex = br.ReadInt32();
                groupsize = br.ReadIntArray(2);
            }

            var blendtype = br.ReadIntArray(2); // paramindex in source
            var blendstart = br.ReadSingleArray(2); // paramstart
            var blendend = br.ReadSingleArray(2); // paramend
            var blendparent = br.ReadInt32(); // paramparent

            var seqgroup = 0;
            if (data.Version == MDLVersionGoldsource)
            {
                seqgroup = br.ReadInt32();
            }

            if (data.Version >= MDLVersionSource2006)
            {
                var fadeintime = br.ReadSingle();
                var fadeouttime = br.ReadSingle();
            }

            var entrynode = br.ReadInt32();
            var exitnode = br.ReadInt32();
            var nodeflags = br.ReadInt32();

            if (data.Version >= MDLVersionSource2006)
            {
                var entryphase = br.ReadSingle();
                var exitphase = br.ReadSingle();

                var lastframe = br.ReadSingle();
            }

            var nextseq = br.ReadInt32();

            if (data.Version >= MDLVersionSource2006)
            {
                var pose = br.ReadInt32();

                var numikrules = br.ReadInt32();

                var numautolayers = br.ReadInt32();
                var autolayerindex = br.ReadInt32();

                var weightlistindex = br.ReadInt32();

                var wlpos = br.BaseStream.Position;
                br.BaseStream.Position = startReadIndex + weightlistindex;
                
                var weightList = br.ReadSingleArray(model.Bones.Count);
                
                br.BaseStream.Position = wlpos;

                var posekeyindex = br.ReadInt32();

                var numiklocks = br.ReadInt32();
                var iklockindex = br.ReadInt32();

                var keyvalueindex = br.ReadInt32();
                var keyvaluesize = br.ReadInt32();

                var cycleposeindex = br.ReadInt32();
                br.ReadIntArray(7); // Unused
            }

            // Load animtion values
            var pos = br.BaseStream.Position;

            if (data.Version == MDLVersionGoldsource)
            {
                if (seqgroup > 0)
                {
                    //TODO: load animations from other files
                    return;
                    // sub out br for another br against the new SG file
                    // br = new BinaryReader(file....etc)
                    // br.BaseStream.Position = animindex;
                }
                br.BaseStream.Position = groups[seqgroup].GroupZeroDataIndex + animindex;
                ReadAnimationGoldsource(br, model, numframes);
            }
            else if (data.Version >= MDLVersionSource2006)
            {
                br.BaseStream.Position = startReadIndex + animindex;
                ReadAnimationSource(br, data, model, groupsize);
            }

            br.BaseStream.Position = pos;
        }

        private static void ReadAnimationSource(BinaryReader br, ModelData data, DataStructures.Models.Model model, int[] groupsize)
        {
            if (groupsize.Length == 0) return;

            var animIndices = br.ReadShortArray(groupsize[0] * groupsize[1]);
            // Just use the first animation for now
            var srcAnim = data.SourceAnimations.FirstOrDefault(x => x.AnimID == animIndices[0]);
            if (srcAnim == null) return;

            var numframes = srcAnim.NumFrames;

            var anim = new Animation();
            // Add all the empty frames
            for (var i = 0; i < numframes; i++) anim.Frames.Add(new AnimationFrame());

            foreach (var bone in model.Bones)
            {
                var animBone = srcAnim.AnimationBones.FirstOrDefault(x => x.Bone == bone.BoneIndex);
                
                for (var f = 0; f < numframes; f++)
                {
                    var fpos = CoordinateF.Zero;
                    var fang = CoordinateF.Zero;

                    if (animBone != null)
                    {
                        if (animBone.FixedPosition != null) fpos = animBone.FixedPosition;
                        else if (animBone.FramePositions.Count > f) fpos = animBone.FramePositions[f];

                        if (animBone.FixedQuaternion == null) fang = animBone.FrameAngles[f];
                    }

                    fpos = fpos.ComponentMultiply(bone.DefaultPositionScale);// +bone.DefaultPosition;
                    var fangq = animBone != null && animBone.FixedQuaternion != null
                                ? animBone.FixedQuaternion
                                : QuaternionF.EulerAngles(fang.ComponentMultiply(bone.DefaultAnglesScale));// + bone.DefaultAngles);
                    anim.Frames[f].Bones.Add(new BoneAnimationFrame(bone, fpos, fangq));
                }
            }

            model.Animations.Add(anim);
        }

        private static void LoadSourceAnimationData(BinaryReader br, ModelData modelData, int numAnim, int animIndex)
        {
            modelData.SourceAnimations = new List<SourceAnimation>();

            var restorePoint = br.BaseStream.Position;
            br.BaseStream.Position = animIndex;

            for (var i = 0; i < numAnim; i++)
            {
                var animStartPos = br.BaseStream.Position;

                var basePointer = br.ReadInt32();
                var szNameIndex = br.ReadInt32();

                var fps = br.ReadSingle();
                var animDescFlags = br.ReadInt32();

                var numframes = br.ReadInt32();

                var numMovements = br.ReadInt32();
                var movementIndex = br.ReadInt32();

                br.ReadCoordinateFArray(2); // bounding box; unused

                var ablock = br.ReadInt32();
                var aindex = br.ReadInt32();

                var numIkRules = br.ReadInt32();
                var ikRuleIndex = br.ReadInt32();

                var animBlockIkRuleIndex = br.ReadInt32();
                br.ReadIntArray(7); // Unused

                var animEndPos = br.BaseStream.Position;

                var sourceAnim = new SourceAnimation(i, numframes, fps, animDescFlags, numMovements, movementIndex,
                                                     ablock, aindex, numIkRules, ikRuleIndex, animBlockIkRuleIndex);

                var currentOffset = aindex;
                short nextOffset;
                do
                {
                    br.BaseStream.Position = animStartPos + currentOffset;
                    var animBone = br.ReadByte();
                    var animFlags = br.ReadByte();
                    nextOffset = br.ReadInt16();
                    currentOffset += nextOffset;

                    var aniBone = new SourceAnimationBone(animBone, animFlags, numframes);
                    aniBone.ReadData(br);
                    sourceAnim.AnimationBones.Add(aniBone);

                } while (nextOffset != 0);

                modelData.SourceAnimations.Add(sourceAnim);

                br.BaseStream.Position = animEndPos;
            }

            br.BaseStream.Position = restorePoint;
        }

        private static void ReadAnimationGoldsource(BinaryReader br, DataStructures.Models.Model model, int numframes)
        {
            var anim = new Animation();
            // Add all the empty frames
            for (var i = 0; i < numframes; i++) anim.Frames.Add(new AnimationFrame());

            // Now we have a reader with the position at the start of the animation data
            // First up is the list of offset indexes for the data, one for each bone in the model.
            // Bones are already loaded up, so loop through those.
            foreach (var bone in model.Bones)
            {
                var offsetPos = br.BaseStream.Position;
                var offsets = br.ReadShortArray(6);
                var restorePoint = br.BaseStream.Position;

                var position = bone.DefaultPosition;
                var angles = bone.DefaultAngles;

                var boneFrames = new List<float[]>();
                for (var i = 0; i < numframes; i++) boneFrames.Add(new float[] {0, 0, 0, 0, 0, 0});

                for (var i = 0; i < 6; i++) // For each offset [X, Y, Z, XR, YR, ZR]
                {
                    if (offsets[i] <= 0) continue;
                    br.BaseStream.Position = offsetPos + offsets[i];
                    var values = ReadRLEEncodedAnimationFrameValues(br, numframes);
                    for (var f = 0; f < numframes; f++)
                    {
                        boneFrames[f][i] += values[f];
                    }
                }

                for (var f = 0; f < numframes; f++)
                {
                    var frame = boneFrames[f];
                    var fpos = new CoordinateF(frame[0], frame[1], frame[2]).ComponentMultiply(bone.DefaultPositionScale) + bone.DefaultPosition;
                    var fang = new CoordinateF(frame[3], frame[4], frame[5]).ComponentMultiply(bone.DefaultAnglesScale) + bone.DefaultAngles;
                    anim.Frames[f].Bones.Add(new BoneAnimationFrame(bone, fpos, QuaternionF.EulerAngles(fang)));
                }

                br.BaseStream.Position = restorePoint;
            }

            model.Animations.Add(anim);
        }

        private static List<short> ReadRLEEncodedAnimationFrameValues(BinaryReader br, int numframes)
        {
            byte valid = 0, total = 0, cindex = 1;
            short lastValue = -1;
            var skip = false;
            var values = new List<short>();
            for (var f = 0; f < numframes; f++)
            {
                if (cindex > total)
                {
                    valid = br.ReadByte();
                    total = br.ReadByte();
                    cindex = 0;
                    if (total == 0)
                    {
                        // zero out the remaining values
                        skip = true;
                        lastValue = valid = cindex = 0;
                    }
                }

                if (cindex < valid) lastValue = br.ReadInt16();
                if (!skip) cindex++;
                values.Add(lastValue);
            }
            return values;
        }

        private static SequenceGroup ReadSequenceGroup(BinaryReader br, ModelData data)
        {
            var name = br.ReadFixedLengthString(Encoding.ASCII, 32);
            var filename = br.ReadFixedLengthString(Encoding.ASCII, 64);
            var cachepointer = br.ReadInt32();
            var groupZeroDataIndex = br.ReadInt32();
            return new SequenceGroup
                       {
                           Name = name,
                           FileName = filename,
                           CachePointer = cachepointer,
                           GroupZeroDataIndex = groupZeroDataIndex
                       };
        }
        #endregion

        #region Bones
        private static void ReadBone(BinaryReader br, int index, ModelData data, DataStructures.Models.Model model)
        {
            var name = "";
            var nameIndex = 0;
            if (data.Version >= MDLVersionSource2006)
            {
                nameIndex = br.ReadInt32();
            }
            else if (data.Version == MDLVersionGoldsource)
            {
                name = br.ReadFixedLengthString(Encoding.UTF8, 32);
            }
            var parent = br.ReadInt32();
            int flags = 0;
            if (data.Version == MDLVersionGoldsource)
            {
                flags = br.ReadInt32();
            }
            var boneController = br.ReadIntArray(6);        // 3 pos, 3 rot
            var defPos = br.ReadCoordinateF();
            QuaternionF quat = null;
            if (data.Version >= MDLVersionSource2006)
            {
                // quaternion
                quat = new QuaternionF(br.ReadCoordinateF(), br.ReadSingle());
            }
            var defAng = br.ReadCoordinateF();
            var defPosScale = br.ReadCoordinateF();
            var defAngScale = br.ReadCoordinateF();

            if (data.Version >= MDLVersionSource2006)
            {
                var poseToBone = br.ReadIntArray(12); // 3x4 matrix
                var qAlignment = new QuaternionF(br.ReadCoordinateF(), br.ReadSingle());
                flags = br.ReadInt32();
                var proctype = br.ReadInt32();
                var procindex = br.ReadInt32();
                var physicsbone = br.ReadInt32();
                var surfacepropidx = br.ReadInt32();
                var contents = br.ReadInt32();
                br.ReadIntArray(8); // Unused
            }

            var parentBone = parent < 0 ? null : model.Bones[parent];
            model.Bones.Add(new Bone(index, parent, parentBone, name, defPos, defAng, defPosScale, defAngScale));
        }
        #endregion

        #region Body Parts/Vertices
        private static void ReadBodyPart(BinaryReader br, int bodyPartIndex, ModelData data, DataStructures.Models.Model model)
        {
            var startIndex = data.Version == MDLVersionGoldsource ? 0 : br.BaseStream.Position;
            var name = "";
            var nameIndex = 0;
            if (data.Version >= MDLVersionSource2006)
            {
                nameIndex = br.ReadInt32();
                var idx = br.BaseStream.Position;
                br.BaseStream.Position = startIndex + nameIndex;
                name = br.ReadNullTerminatedString();
                br.BaseStream.Position = idx;
            }
            else if (data.Version == MDLVersionGoldsource)
            {
                name = br.ReadFixedLengthString(Encoding.UTF8, 64);
            }
            var numModels = br.ReadInt32();
            var baseIndex = br.ReadInt32();
            var modelIndex = br.ReadInt32();

            var endIndex = br.BaseStream.Position;

            br.BaseStream.Position = modelIndex + startIndex;
            for (var i = 0; i < numModels; i++)
            {
                ReadStudioModel(br, name, bodyPartIndex, i, data, model);
            }

            br.BaseStream.Position = endIndex;
        }

        private static void ReadStudioModel(BinaryReader br, string groupName, int bodyPartIndex, int modelIndex, ModelData data, DataStructures.Models.Model model)
        {
            var startModelPos = br.BaseStream.Position;
            var name = br.ReadFixedLengthString(Encoding.ASCII, 64);
            var type = br.ReadInt32();
            var radius = br.ReadSingle();

            var numMesh = br.ReadInt32();
            var meshIndex = br.ReadInt32();

            var numVerts = br.ReadInt32();
            var vertInfoIndex = 0;
            if (data.Version == MDLVersionGoldsource)
            {
                vertInfoIndex = br.ReadInt32();
            }
            var vertIndex = br.ReadInt32();
            int numNorms = 0, normInfoIndex = 0, normIndex = 0;
            if (data.Version == MDLVersionGoldsource)
            {
                numNorms = br.ReadInt32();
                normInfoIndex = br.ReadInt32();
                normIndex = br.ReadInt32();
            }
            else if (data.Version >= MDLVersionSource2006)
            {
                var tangentsIndex = br.ReadInt32();
            }

            var numGroups = br.ReadInt32(); // Attachments in source
            var groupIndex = br.ReadInt32(); // Attachments

            if (data.Version >= MDLVersionSource2006)
            {
                var numEyeballs = br.ReadInt32();
                var eyeballIndex = br.ReadInt32();

                var vertexDataPointer = br.ReadInt32();
                var tangentDataPointer = br.ReadInt32();

                br.ReadIntArray(8); // Unused
            }

            var endPos = br.BaseStream.Position;

            if (data.Version == MDLVersionGoldsource)
            {
                ReadVerticesGoldSource(br, groupName, modelIndex, model, numVerts, vertInfoIndex, vertIndex, numMesh, meshIndex, numNorms, normInfoIndex, normIndex);
            }
            else if (data.Version >= MDLVersionSource2006)
            {
                ReadVerticesSource(br, groupName, bodyPartIndex, modelIndex, data, model, numMesh, startModelPos + meshIndex);
            }

            br.BaseStream.Position = endPos;
        }

        private static void ReadVerticesGoldSource(BinaryReader br, string bodyPartName, int modelIndex, DataStructures.Models.Model model, int numVerts, int vertInfoIndex, int vertIndex, int numMesh, int meshIndex, int numNorms, int normInfoIndex, int normIndex)
        {
            br.BaseStream.Position = vertInfoIndex;
            var vertInfoData = br.ReadByteArray(numVerts);
            br.BaseStream.Position = normInfoIndex;
            var normInfoData = br.ReadByteArray(numNorms);
            br.BaseStream.Position = vertIndex;
            var vertices = br.ReadCoordinateFArray(numVerts);
            br.BaseStream.Position = normIndex;
            var normals = br.ReadCoordinateFArray(numNorms);

            br.BaseStream.Position = meshIndex;
            for (var i = 0; i < numMesh; i++)
            {
                var mesh = new Mesh(0); // GoldSource meshes don't have LODs
                var meshNumTris = br.ReadInt32();
                var meshTriIndex = br.ReadInt32();
                var meshSkinRef = br.ReadInt32();
                var meshNumNorms = br.ReadInt32();
                var meshNormIndex = br.ReadInt32();

                mesh.SkinRef = meshSkinRef;

                var pos = br.BaseStream.Position;
                br.BaseStream.Position = meshTriIndex;
                int sh;
                // Read all the triangle strips and fans from the mesh and convert into easy-to-render 3-point triangles
                while ((sh = br.ReadInt16()) != 0)
                {
                    var list = new List<MdlProviderSequenceDataPoint>();
                    var fan = sh < 0;
                    if (fan) sh = -sh; // Negative value flags a fan, otherwise it is a strip
                    for (var j = 0; j < sh; j++) // Read the points in the sequence
                    {
                        list.Add(new MdlProviderSequenceDataPoint
                                     {
                                         Vertex = br.ReadInt16(), // Vertex index in the vertices array
                                         Normal = br.ReadInt16(), // Normal index in the normals array
                                         TextureS = br.ReadInt16(),
                                         TextureT = br.ReadInt16()
                                     });
                    }
                    for (var j = 0; j < list.Count - 2; j++)
                    {
                        // Get the vert indices to use for the various types of strip/fan
                        //                    |TRIANGLE FAN   |                       |TRIANGLE STRIP (ODD)|         |TRIANGLE STRIP (EVEN)|
                        var add = fan ? new[] {0, j + 1, j + 2} : (j % 2 == 1 ? new[] {j + 1, j, j + 2     } : new[] {j, j + 1, j + 2      });
                        foreach (var idx in add)
                        {
                            var vi = list[idx];
                            var boneIndex = vertInfoData[vi.Vertex]; // Vertinfo tells what bone the vert belongs to
                            mesh.Vertices.Add(new MeshVertex(
                                vertices[vi.Vertex],
                                normals[vi.Normal],
                                model.Bones[boneIndex],
                                vi.TextureS,
                                vi.TextureT));
                        }
                    }
                }
                model.AddMesh(bodyPartName, modelIndex, mesh);
                br.BaseStream.Position = pos;
            }
        }

        private static void ReadVerticesSource(BinaryReader br, string groupName, int bodyPartIndex, int modelIndex, ModelData modelData, DataStructures.Models.Model model, int numMesh, long meshIndex)
        {
            br.BaseStream.Position = meshIndex;
            for (var i = 0; i < numMesh; i++)
            {
                var material = br.ReadInt32();
                var modelOffset = br.ReadInt32();
                var numVerts = br.ReadInt32();
                var vertexOffset = br.ReadInt32();
                var numFlexes = br.ReadInt32();
                var flexIndex = br.ReadInt32();
                var materialType = br.ReadInt32();
                var materialParam = br.ReadInt32();
                var meshId = br.ReadInt32();
                var center = br.ReadCoordinateF();
                var modelVertexDataPointer = br.ReadInt32();
                var numLODVertices = br.ReadIntArray(8);
                br.ReadIntArray(8); // Unused

                foreach (var mm in modelData.Meshes.Where(mm => mm.BodyPart == bodyPartIndex
                                                                && mm.Model == modelIndex
                                                                && mm.LOD == 0
                                                                && mm.MeshIndex == meshId))
                {
                    var mesh = new Mesh(mm.LOD);
                    foreach (var point in mm.Mesh.Points)
                    {
                        var vert = modelData.Vertices[point.VertexIndex + vertexOffset];
                        var boneWeights = new List<BoneWeighting>();
                        for (var j = 0; j < vert.NumBones; j++)
                        {
                            boneWeights.Add(new BoneWeighting(model.Bones[vert.Bones[j]], vert.BoneWeights[j]));
                        }
                        var mv = new MeshVertex(vert.Position, vert.Normal, boneWeights, vert.TextureS, vert.TextureT);
                        mesh.Vertices.Add(mv);
                    }
                    model.AddMesh(groupName, modelIndex, mesh);
                }
            }
        }
        #endregion

        #region LoadSourceMeshData
        private static void LoadSourceMeshData(ModelData modelData, IFile file)
        {
            modelData.Meshes = new List<VTXModel>();
            // In Source the vertices are saved to the VVD file
            // The vertex windings are saved in the VTX file
            var vvd = file.GetRelatedFile("vvd");
            var vtx = file.GetRelatedFile("vtx");
            if (vvd == null) throw new ProviderException("Unable to locate " + file.NameWithoutExtension + ".vvd");
            if (vtx == null) throw new ProviderException("Unable to locate " + file.NameWithoutExtension + ".vtx");

            var vertices = new List<VVDPoint>();

            using (var fs = vvd.Open())
            {
                using (var vbr = new BinaryReader(fs))
                {
                    var magicString = vbr.ReadFixedLengthString(Encoding.UTF8, 4);
                    if (magicString != MagicStringIDSV)
                    {
                        throw new ProviderException("Bad magic number for vertex file. Expected IDSV, got: " + magicString);
                    }

                    var version = vbr.ReadInt32();
                    if (version != VVDVersionSource)
                    {
                        throw new ProviderException("Bad version number for vertex file. Expected 4, got: " + version);
                    }

                    long checksum = vbr.ReadInt32();
                    var numLods = vbr.ReadInt32();
                    var numLodVertices = vbr.ReadIntArray(8);

                    var numFixups = vbr.ReadInt32();
                    var fixupTableStart = vbr.ReadInt32();
                    var vertexDataStart = vbr.ReadInt32();
                    var tangentDataStart = vbr.ReadInt32();

                    vbr.BaseStream.Position = vertexDataStart;

                    // Read all the vertices from LOD 0 (this should contain the vertices for all LODs)
                    for (var i = 0; i < numLodVertices[0]; i++)
                    {
                        var boneWeights = vbr.ReadSingleArray(3);
                        var bones = vbr.ReadBytes(3);
                        var numBones = vbr.ReadByte();
                        var position = vbr.ReadCoordinateF();
                        var normal = vbr.ReadCoordinateF();
                        var textureS = vbr.ReadSingle();
                        var textureT = vbr.ReadSingle();
                        vertices.Add(new VVDPoint(boneWeights, bones, numBones, position, normal, textureS, textureT));
                    }

                    // Apply the fixup table, this re-orders the indices in reverse LOD order for performance reasons
                    if (numFixups > 0)
                    {
                        vbr.BaseStream.Position = fixupTableStart;
                        var newVerts = new List<VVDPoint>();
                        for (var i = 0; i < numFixups; i++)
                        {
                            var fuLod = vbr.ReadInt32();
                            var fuvertid = vbr.ReadInt32();
                            var funumverts = vbr.ReadInt32();
                            newVerts.AddRange(vertices.GetRange(fuvertid, funumverts));
                        }
                        vertices.Clear();
                        vertices.AddRange(newVerts);
                    }

                    modelData.Vertices = vertices;
                }
            }
            using (var fs = vtx.Open())
            {
                using (var vbr = new BinaryReader(fs))
                {
                    var version = vbr.ReadInt32(); // 7
                    if (version != VTXVersionSource)
                    {
                        throw new ProviderException("Bad version number for vertex file. Expected 7, got: " + version);
                    }
                    var vertCacheSize = vbr.ReadInt32();
                    var maxBonesPerStrip = vbr.ReadUInt16();
                    var maxBonesPerTri = vbr.ReadUInt16();
                    var maxBonesPerVert = vbr.ReadInt32();
                    long checksum = vbr.ReadInt32();
                    var numLods = vbr.ReadInt32();
                    var materialReplacementListOffset = vbr.ReadInt32();
                    var numBodyParts = vbr.ReadInt32();
                    var bodyPartOffset = vbr.ReadInt32();

                    // BODY PARTS
                    long posbp = bodyPartOffset;
                    for (var bp = 0; bp < numBodyParts; bp++)
                    {
                        vbr.BaseStream.Position = posbp;

                        var numModels = vbr.ReadInt32();
                        var modelOffset = vbr.ReadInt32();

                        var posmdl = posbp + modelOffset;
                        posbp = vbr.BaseStream.Position;

                        // MODELS
                        for (var mdl = 0; mdl < numModels; mdl++)
                        {
                            vbr.BaseStream.Position = posmdl;

                            var numLod = vbr.ReadInt32();
                            var lodOffset = vbr.ReadInt32();

                            var poslod = posmdl + lodOffset;
                            posmdl = vbr.BaseStream.Position;

                            // LODS
                            for (var lod = 0; lod < numLod; lod++)
                            {
                                vbr.BaseStream.Position = poslod;

                                var meshNum = vbr.ReadInt32();
                                var meshOffset = vbr.ReadInt32();
                                var switchPoint = vbr.ReadSingle();

                                var posmesh = poslod + meshOffset;
                                poslod = vbr.BaseStream.Position;

                                // MESHES
                                for (var msh = 0; msh < meshNum; msh++)
                                {
                                    vbr.BaseStream.Position = posmesh;

                                    var sgNum = vbr.ReadInt32();
                                    var sgOffset = vbr.ReadInt32();
                                    var meshFlags = vbr.ReadByte();

                                    var possg = posmesh + sgOffset;
                                    posmesh = vbr.BaseStream.Position;

                                    var mesh = new VTXModel(bp, mdl, lod, msh);

                                    // STRIP GROUPS
                                    for (var sg = 0; sg < sgNum; sg++)
                                    {
                                        vbr.BaseStream.Position = possg;

                                        var vertNum = vbr.ReadInt32();
                                        var vertOffset = vbr.ReadInt32();
                                        var indexNum = vbr.ReadInt32();
                                        var indexOffset = vbr.ReadInt32();
                                        var stripNum = vbr.ReadInt32();
                                        var stripOffset = vbr.ReadInt32();
                                        var sgFlags = vbr.ReadByte();
                                        // vbr.ReadIntArray(2); //TODO FIXME Newer model format 49's (DOTA2, CSGO) have two extra integers here, (num + offset, purpose unknown)

                                        var posvert = possg + vertOffset;
                                        var posidx = possg + indexOffset;
                                        var posstrip = possg + stripOffset;
                                        possg = vbr.BaseStream.Position;

                                        var vertinfo = new List<VTXPoint>();
                                        vbr.BaseStream.Position = posvert;
                                        for (var vert = 0; vert < vertNum; vert++)
                                        {
                                            var boneWeightIndices = vbr.ReadBytes(3);
                                            var numBones = vbr.ReadByte();
                                            var meshVertex = vbr.ReadInt16();
                                            var boneIDs = vbr.ReadBytes(3);

                                            vertinfo.Add(new VTXPoint(boneWeightIndices, numBones, meshVertex, boneIDs));
                                        }

                                        vbr.BaseStream.Position = posidx;
                                        var indices = vbr.ReadShortArray(indexNum);

                                        // The strips hold info about whether this is a triangle strip or just a list
                                        vbr.BaseStream.Position = posstrip;
                                        for (var st = 0; st < stripNum; st++)
                                        {
                                            var numStIndices = vbr.ReadInt32();
                                            var stIndexOffset = vbr.ReadInt32();
                                            var numStVerts = vbr.ReadInt32();
                                            var stVertOffset = vbr.ReadInt32();
                                            var numStBones = vbr.ReadInt16();
                                            var stFlags = vbr.ReadByte();
                                            var numStBoneStateChanges = vbr.ReadInt32();
                                            var stBoneStateChangeOffset = vbr.ReadInt32();
                                            // vbr.ReadIntArray(2); //TODO FIXME Newer model format 49's (DOTA2, CSGO) have two extra integers here, (num + offset, purpose unknown)

                                            if ((stFlags & VTXStripGroupTriListFlag) > 0)
                                            {
                                                for (var j = stIndexOffset; j < stIndexOffset + numStIndices; j++)
                                                {
                                                    mesh.Mesh.Points.Add(vertinfo[indices[j]]);
                                                    //mesh.Vertices.Add(vertices[vertinfo[indices[j]]]);
                                                }
                                            }
                                            else if ((stFlags & VTXStripGroupTriStripFlag) > 0)
                                            {
                                                for (var j = stIndexOffset; j < stIndexOffset + numStIndices - 2; j++)
                                                {
                                                    var add = j % 2 == 1 ? new[] { j + 1, j, j + 2 } : new[] { j, j + 1, j + 2 };
                                                    foreach (var idx in add)
                                                    {
                                                        mesh.Mesh.Points.Add(vertinfo[indices[idx]]);
                                                        //mesh.Vertices.Add(vertices[vertinfo[indices[idx]]]);
                                                    }
                                                }
                                            }
                                        } // Strips
                                    } // Strip Groups
                                    modelData.Meshes.Add(mesh);
                                } // Meshes
                            } // LODs
                        } // Models
                    } // Body Parts
                } // using (var br)
            } // using (var fs)
        }
        #endregion

        #region Data Structures
        private class ModelData
        {
            public int Version { get; set; }
            public List<VTXModel> Meshes { get; set; }
            public List<VVDPoint> Vertices { get; set; }
            public List<SourceAnimation> SourceAnimations { get; set; }
        }

        private class VTXModel
        {
            public int BodyPart { get; private set; }
            public int Model { get; private set; }
            public int LOD { get; private set; }
            public int MeshIndex { get; private set; }
            public VTXMesh Mesh { get; private set; }

            public VTXModel(int bodyPart, int model, int lod, int meshIndex)
            {
                BodyPart = bodyPart;
                Model = model;
                LOD = lod;
                MeshIndex = meshIndex;
                Mesh = new VTXMesh();
            }
        }

        private class VTXMesh
        {
            public List<VTXPoint> Points { get; private set; }

            public VTXMesh()
            {
                Points = new List<VTXPoint>();
            }
        }

        private class VTXPoint
        {
            public byte[] BoneWeightIndices { get; private set; }
            public int NumBones { get; private set; }
            public short VertexIndex { get; private set; }
            public byte[] BoneIDs { get; private set; }

            public VTXPoint(byte[] boneWeightIndices, int numBones, short vertexIndex, byte[] boneIDs)
            {
                BoneWeightIndices = boneWeightIndices;
                NumBones = numBones;
                VertexIndex = vertexIndex;
                BoneIDs = boneIDs;
            }
        }

        private class VVDPoint
        {
            public float[] BoneWeights { get; private set; }
            public byte[] Bones { get; private set; }
            public int NumBones { get; private set; }
            public CoordinateF Position { get; private set; }
            public CoordinateF Normal { get; private set; }
            public float TextureS { get; private set; }
            public float TextureT { get; private set; }

            public VVDPoint(float[] boneWeights, byte[] bones, int numBones,
                CoordinateF position, CoordinateF normal,
                float textureS, float textureT)
            {
                BoneWeights = boneWeights;
                Bones = bones;
                NumBones = numBones;
                Position = position;
                Normal = normal;
                TextureS = textureS;
                TextureT = textureT;
            }
        }

        private class SourceAnimation
        {
            public int AnimID { get; set; }
            public int NumFrames { get; set; }
            public float FPS { get; set; }
            public int Flags { get; set; }
            public int NumMovements { get; private set; }
            public int MovementIndex { get; private set; }
            public int AnimBlock { get; private set; }
            public int AnimIndex { get; private set; }
            public int NumIKRules { get; private set; }
            public int IKRuleIndex { get; private set; }
            public int AnimBlockIKRuleIndex { get; private set; }
            public List<SourceAnimationBone> AnimationBones { get; private set; }

            public SourceAnimation(int id, int numFrames, float fps, int flags, int numMovements, int movementIndex, int animBlock, int animIndex, int numIKRules, int ikRuleIndex, int animBlockIKRuleIndex)
            {
                AnimID = id;
                NumFrames = numFrames;
                FPS = fps;
                Flags = flags;
                NumMovements = numMovements;
                MovementIndex = movementIndex;
                AnimBlock = animBlock;
                AnimIndex = animIndex;
                NumIKRules = numIKRules;
                IKRuleIndex = ikRuleIndex;
                AnimBlockIKRuleIndex = animBlockIKRuleIndex;
                AnimationBones = new List<SourceAnimationBone>();
            }
        }

        private class SourceAnimationBone
        {
            private const byte StudioAnimRawpos = 0x01;
            private const byte StudioAnimRawrot = 0x02;
            private const byte StudioAnimAnimpos = 0x04;
            private const byte StudioAnimAnimrot = 0x08;
            private const byte StudioAnimDelta = 0x10;

            const float Half = 0x8000;
            const float Quarter = 0x4000;

            public byte Bone { get; private set; }
            public byte Flags { get; private set; }
            public int NumFrames { get; private set; }
            public QuaternionF FixedQuaternion { get; set; }
            public CoordinateF FixedPosition { get; private set; }
            public List<CoordinateF> FrameAngles { get; private set; }
            public List<CoordinateF> FramePositions { get; private set; }

            public SourceAnimationBone(byte bone, byte flags, int numFrames)
            {
                Bone = bone;
                Flags = flags;
                NumFrames = numFrames;
                FixedPosition = null;
                FixedQuaternion = null;
                FramePositions = new List<CoordinateF>();
                FrameAngles = new List<CoordinateF>();
            }

            public void ReadData(BinaryReader br)
            {
                var delta = (Flags & StudioAnimDelta) > 0;
                if ((Flags & StudioAnimRawrot) > 0)
                {
                    // WTF is this messy format :|
                    // 48-bit Quaternion: 16-bit x, 16-bit y, 15-bit z, 1 bit to flag if w is negative
                    // Convert into real quaternion with the algorithm below (taken from compressed_vector.h)
                    int x = br.ReadUInt16();
                    int y = br.ReadUInt16();
                    var temp = br.ReadUInt16();
                    var z = temp & 0x7FFF; // Get the last 15 bits from the short
                    var isWneg = (temp & 0x8000) > 0; // The first bit is the boolean value
                    var w = (isWneg ? -1 : 1) * (float) Math.Sqrt(1 - x * x - y * y - z * z);
                    FixedQuaternion = new QuaternionF((x - Half) / Half, (y - Half) / Half, (z - Quarter) / Quarter, w);
                }
                if ((Flags & StudioAnimRawpos) > 0)
                {
                    // What's this? A custom made, 16-bit floating point implementation? WHAT DID I DO TO DESERVE THIS???
                    var bytes = br.ReadBytes(6);
                    // Wait a minute.....
                    var x = OpenTK.Half.FromBytes(bytes, 0).ToSingle();
                    var y = OpenTK.Half.FromBytes(bytes, 2).ToSingle();
                    var z = OpenTK.Half.FromBytes(bytes, 4).ToSingle();
                    // Ha ha, screw you, custom floating-point implementation! Thanks, OpenTK!
                    FixedPosition = new CoordinateF(x, y, z);
                }
                if ((Flags & StudioAnimAnimrot) > 0)
                {
                    // Why is this so painful :(
                    // Read the per-frame data using RLE, just like GoldSource models
                    var startPos = br.BaseStream.Position;
                    var offsets = br.ReadShortArray(3);
                    var endPos = br.BaseStream.Position;
                    var rotFrames = new List<float[]>();
                    for (var i = 0; i < NumFrames; i++) rotFrames.Add(new float[] {0, 0, 0});
                    for (var i = 0; i < 3; i++)
                    {
                        if (offsets[i] == 0) continue;
                        br.BaseStream.Position = startPos + offsets[i];
                        var values = ReadRLEEncodedAnimationFrameValues(br, NumFrames);
                        for (var f = 0; f < values.Count; f++)
                        {
                            rotFrames[f][i] =+ values[f];
                            if (f > 0 && delta) rotFrames[f][i] += values[f - 1];
                        }
                    }
                    FrameAngles.AddRange(rotFrames.Select(x => new CoordinateF(x[0], x[1], x[2])));
                    br.BaseStream.Position = endPos;
                }
                if ((Flags & StudioAnimAnimpos) > 0)
                {
                    // Same as above, except for the position coordinate
                    var startPos = br.BaseStream.Position;
                    var offsets = br.ReadShortArray(3);
                    var endPos = br.BaseStream.Position;
                    var posFrames = new List<float[]>();
                    for (var i = 0; i < NumFrames; i++) posFrames.Add(new float[] { 0, 0, 0 });
                    for (var i = 0; i < 3; i++)
                    {
                        if (offsets[i] == 0) continue;
                        br.BaseStream.Position = startPos + offsets[i];
                        var values = ReadRLEEncodedAnimationFrameValues(br, NumFrames);
                        for (var f = 0; f < values.Count; f++)
                        {
                            posFrames[f][i] = +values[f];
                            if (f > 0 && delta) posFrames[f][i] += values[f - 1];
                        }
                    }
                    FramePositions.AddRange(posFrames.Select(x => new CoordinateF(x[0], x[1], x[2])));
                    br.BaseStream.Position = endPos;
                }
            }
        }

        private class MdlProviderSequenceDataPoint
        {
            public short Vertex { get; set; }
            public short Normal { get; set; }
            public short TextureS { get; set; }
            public short TextureT { get; set; }
        }

        private class SequenceGroup
        {
            public string Name { get; set; }
            public string FileName { get; set; }
            public int CachePointer { get; set; }
            public int GroupZeroDataIndex { get; set; }
        }
        #endregion
    }

    public interface IModelTextureInfo
    {
        
    }
}
