using Microsoft.Xna.Framework.Graphics;
using SkinnedModelData;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MadScienceLab
{
    public class GameAnimatedModel : CellObject
    {
        private CellObject parent;

        private string _assetFile;
        private AnimationPlayer _animationPlayer;
        private SkinningData _skinningData;
        private float _speedScale = 1f;
        
        private string _initClipName;
        private bool _initLoop;
        private float _initBlendTime;

        public int VerticalOffset { get; set; }

        /**
         * Matt: Needs to take parent now, so it can move with it (mainly for player)
         */
        public GameAnimatedModel(string assetFile, int startRow, int startCol, CellObject parent):base(startRow, startCol)
        {
            this.parent = parent;
            _assetFile = assetFile;
            VerticalOffset = 0;
        }


        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);

            Model = contentManager.Load<Model>(_assetFile);

            _skinningData = Model.Tag as SkinningData;
            
            Debug.Assert(_skinningData != null, "Model (" + _assetFile + ") contains no Skinning Data!");

            _animationPlayer = new AnimationPlayer(_skinningData);
            _animationPlayer.SetAnimationSpeed(_speedScale);

            if (_initClipName != null)
            {
                PlayAnimation(_initClipName, _initLoop, _initBlendTime);
            }
        }

        public void SetAnimationSpeed(float speedScale)
        {
            if (_animationPlayer != null)
                _animationPlayer.SetAnimationSpeed(speedScale);

            _speedScale = speedScale;
        }


        public void PlayAnimation(string clipName, bool loop, float blendTime)
        {
            if (_animationPlayer == null)
            {
                _initClipName = clipName;
                _initLoop = loop;
                _initBlendTime = blendTime;
                return;
            }

            var clip = _skinningData.AnimationClips[clipName];
            if (clip != null) _animationPlayer.StartClip(clip, loop, blendTime);
        }

        public void PlayAnimationOnceNoLoop(string clipName, float blendTime)
        {
            if (_animationPlayer == null)
            {
                _initClipName = clipName;
                _initLoop = false;
                _initBlendTime = blendTime;
                return;
            }

            var clip = _skinningData.AnimationClips[clipName];
            if (clip != null) _animationPlayer.PlayClipOnceNoLoop(clip, blendTime);
        }

        public void StopAnimation()
        {
            _animationPlayer.StopClip();
        }

        public Matrix GetBoneTransform(string boneName)
        {
            if (_animationPlayer != null)
                return _animationPlayer.GetBoneTransform(boneName);

            return Matrix.Identity;
        }

        public override void Update(RenderContext renderContext)
        {            
            /*
             * Matt: there is probably a more efficient way to do this, but my Matrix math is awful. 
             */
            WorldMatrix = Matrix.CreateFromQuaternion(parent.LocalRotation) *
                          Matrix.CreateScale(parent.LocalScale) *
                          Matrix.CreateTranslation(new Vector3(parent.Position.X, parent.Position.Y - VerticalOffset, parent.Position.Z));

            _animationPlayer.Update(renderContext.GameTime.ElapsedGameTime, true, WorldMatrix);
        }

        public override void Draw(RenderContext renderContext)
        {
            Matrix[] bones = null;
            bones = _animationPlayer.GetSkinTransforms();

            // Render the skinned mesh.
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);

                    effect.EnableDefaultLighting();
                    effect.AmbientLightColor = new Vector3(.3f, .3f, .3f);
                    effect.View = renderContext.Camera.View;
                    effect.Projection = renderContext.Camera.Projection;

                    effect.SpecularColor = new Vector3(1,1,1);
                    effect.SpecularPower = 200;
                }

                mesh.Draw();
            }
        }
    }
}
