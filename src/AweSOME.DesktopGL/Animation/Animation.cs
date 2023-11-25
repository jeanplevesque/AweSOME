using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using AweSOME;


namespace AwesomeAnimation
{
    public enum AnimationSens{Reverse=-1,Normal=1}
    public enum AnimationTypes { Corps, Jambes, Pieds, Bras, Mains, Nombre }
    public enum AnimationPlayModes { Loop, Once, Nombre }

    class Animation
    {
        //public AnimationFrame[] ListeFrame;
        public string Nom="defaut";
        public List<AnimationFrame> ListeFrames = new List<AnimationFrame>();

        public int Index
        {
            get { return index_; }
            set
            {
                switch(AnimationPlayMode)
                {
                    case AnimationPlayModes.Loop:
                        index_ = (value + NbFrames )% NbFrames;
                        Progression = 0;
                        break;
                    case AnimationPlayModes.Once:
                        if (value<NbFrames)
                        {
                            index_ = value;
                            Progression = 0;
                        }
                        else { EstTerminée = true; }                   
                        break;
                }
            }
        }
        int index_;
        public int NbFrames;// { get { return ListeFrames.Count; } }
        public float Progression=2;
        public bool EstTerminée;
        public bool Pause;
        public AnimationFrame FrameActive { get { return ListeFrames[Index]; } }
        public AnimationPlayModes AnimationPlayMode;
        public AnimationTypes AnimationType;

        public Animation(int nbFrames,AnimationTypes typeAnimation,AnimationPlayModes playMode)
        {
            //ListeFrame = new AnimationFrame[nbFrames];
            AnimationType = typeAnimation;
            AnimationPlayMode = playMode;
            NbFrames = nbFrames;
        }
        public Animation(int nbFrames)
        {
            //ListeFrame = new AnimationFrame[nbFrames];
            AnimationType = AnimationTypes.Corps;
            AnimationPlayMode = AnimationPlayModes.Loop;
            NbFrames = nbFrames;
        }

        public void AjouterFrame(AnimationFrame frame)
        {
            ListeFrames.Add(frame);            
        }

        public void Reset()
        {
            Index = 0;
            EstTerminée = false;
        }
        public void PlacerSurFrame(Joint[] joints,Vector2 positionCorps,float grosseur,int indexFrame)
        {
            indexFrame = (indexFrame + NbFrames) % NbFrames;
            for (int i = 0; i < joints.Length; ++i)
            {
                joints[i].Position = positionCorps + ListeFrames[indexFrame].ListePositions[i] * grosseur;
                joints[i].PositionVoulue = positionCorps + ListeFrames[indexFrame].ListePositions[i] * grosseur;
                joints[i].AnciennePositionVoulue = positionCorps + ListeFrames[indexFrame].ListePositions[i] * grosseur;
            }

        }
        public void Animer(Personnage perso,float vitesse,AnimationSens sens)
        {
            switch(AnimationType)
            {
                case AnimationTypes.Corps:
                    Animer(perso.ListeJoints.ToArray(), perso.Position,perso.Grosseur, vitesse,(int)perso.Direction,(int)sens);
                    break;
                case AnimationTypes.Jambes:
                    Animer(perso.ListeJointsJambes, perso.Position,perso.Grosseur, vitesse,(int)perso.Direction,(int)sens);
                    break;
                case AnimationTypes.Bras:
                    Animer(perso.ListeJointsBras, perso.Position, perso.Grosseur, vitesse, (int)perso.Direction, (int)sens);
                    break;
                case AnimationTypes.Pieds:
                    Animer(perso.ListeJointsPieds, perso.Position, perso.Grosseur, vitesse, (int)perso.Direction, (int)sens);
                    break;
                case AnimationTypes.Mains:
                    Animer(perso.ListeJointsMains, perso.Position, perso.Grosseur, vitesse, (int)perso.Direction, (int)sens);
                    break;
            }
        }
        public void Animer(Joint[] joints, Vector2 positionCorps,float grosseur ,float vitesse, int direction,int deltaIndex)
        {
            if (!EstTerminée  && !Pause)
            {
                if (Progression >= 1)
                {
                    Index+=deltaIndex;
                    for (int i = 0; i < joints.Length; ++i)
                    {
                        joints[i].Position = joints[i].PositionVoulue;
                        joints[i].AnciennePositionVoulue = joints[i].PositionVoulue;
                        joints[i].PositionVoulue.X = positionCorps.X + ListeFrames[Index].ListePositions[i].X * grosseur * direction;
                        joints[i].PositionVoulue.Y = positionCorps.Y + ListeFrames[Index].ListePositions[i].Y * grosseur;
                    }
                }
                else
                {
                    for (int i = 0; i < joints.Length; ++i)
                    {
                        joints[i].Position = joints[i].AnciennePositionVoulue + (joints[i].PositionVoulue - joints[i].AnciennePositionVoulue) * Progression;
                    }
                    Progression += vitesse;
                }
            }
        }

        

        public void Save(BinaryWriter writer)
        {
            writer.Write(Nom);
            writer.Write(NbFrames);
            foreach (AnimationFrame a in ListeFrames)
            {
                a.Save(writer);
            }
            writer.Write((int)AnimationType);
            writer.Write((int)AnimationPlayMode);
        }
        public static Animation Load(BinaryReader reader)
        {
            string nom = reader.ReadString();
            Animation animation=new Animation(reader.ReadInt32());
            for (int i = 0; i < animation.NbFrames; ++i)
            {
                animation.AjouterFrame(AnimationFrame.Load(reader));
            }
            animation.Nom = nom;
            animation.AnimationType = (AnimationTypes)reader.ReadInt32();
            animation.AnimationPlayMode = (AnimationPlayModes)reader.ReadInt32();
            return animation;
        }

        /// <param name="animationCorps">l'animation de base</param>
        /// <param name="prochainType">le type de la prochaine animation (Envoyez le type de l'animation de base pour un clonage)</param>
        public static Animation CréerÀPartirDeBody(Animation animationCorps, AnimationTypes prochainType)
        {
            Animation prochaineAnimation = null;
            AnimationFrame frame = null;
            if (animationCorps.AnimationType == AnimationTypes.Corps)
            {
                switch (prochainType)
                {
                    case AnimationTypes.Jambes:
                        prochaineAnimation = new Animation(animationCorps.NbFrames);
                        prochaineAnimation.AnimationType = AnimationTypes.Jambes;
                        for (int i = 0; i < prochaineAnimation.NbFrames; ++i)
                        {
                            frame = new AnimationFrame(5);
                            frame.AjouterPosition(animationCorps.ListeFrames[i].ListePositions[0]);
                            frame.AjouterPosition(animationCorps.ListeFrames[i].ListePositions[2]);
                            frame.AjouterPosition(animationCorps.ListeFrames[i].ListePositions[3]);
                            frame.AjouterPosition(animationCorps.ListeFrames[i].ListePositions[1]);
                            frame.AjouterPosition(animationCorps.ListeFrames[i].ListePositions[6]);
                            prochaineAnimation.AjouterFrame(frame);
                        }
                        break;
                    case AnimationTypes.Bras:
                        prochaineAnimation = new Animation(animationCorps.NbFrames);
                        prochaineAnimation.AnimationType = AnimationTypes.Bras;
                        for (int i = 0; i < prochaineAnimation.NbFrames; ++i)
                        {
                            frame = new AnimationFrame(5);
                            frame.AjouterPosition(animationCorps.ListeFrames[i].ListePositions[1]);
                            frame.AjouterPosition(animationCorps.ListeFrames[i].ListePositions[4]);
                            frame.AjouterPosition(animationCorps.ListeFrames[i].ListePositions[5]);
                            frame.AjouterPosition(animationCorps.ListeFrames[i].ListePositions[6]);
                            prochaineAnimation.AjouterFrame(frame);
                        }
                        break;
                    case AnimationTypes.Mains:
                        prochaineAnimation = new Animation(animationCorps.NbFrames);
                        prochaineAnimation.AnimationType = AnimationTypes.Mains;
                        for (int i = 0; i < prochaineAnimation.NbFrames; ++i)
                        {
                            frame = new AnimationFrame(2);
                            frame.AjouterPosition(animationCorps.ListeFrames[i].ListePositions[4]);
                            frame.AjouterPosition(animationCorps.ListeFrames[i].ListePositions[5]);
                            prochaineAnimation.AjouterFrame(frame);
                        }
                        break;
                    case AnimationTypes.Pieds:
                        prochaineAnimation = new Animation(animationCorps.NbFrames);
                        prochaineAnimation.AnimationType = AnimationTypes.Pieds;
                        for (int i = 0; i < prochaineAnimation.NbFrames; ++i)
                        {
                            frame = new AnimationFrame(2);
                            frame.AjouterPosition(animationCorps.ListeFrames[i].ListePositions[2]);
                            frame.AjouterPosition(animationCorps.ListeFrames[i].ListePositions[3]);
                            prochaineAnimation.AjouterFrame(frame);
                        }
                        break;
                    default://------------------------CLONAGE----------------------------
                        prochaineAnimation = new Animation(animationCorps.NbFrames);
                        prochaineAnimation.AnimationType = AnimationTypes.Corps;
                        for (int i = 0; i < prochaineAnimation.NbFrames; ++i)
                        {
                            frame = new AnimationFrame(animationCorps.ListeFrames.Count);
                            for (int j = 0; j < animationCorps.ListeFrames[0].ListePositions.Length; ++j)
                            {
                                frame.AjouterPosition(animationCorps.ListeFrames[i].ListePositions[j]);
                            }
                            prochaineAnimation.AjouterFrame(frame);
                        }
                        break;//---------------------------------------------------------
                }
            }
            else
            {
                return animationCorps;
            }
            prochaineAnimation.AnimationPlayMode = animationCorps.AnimationPlayMode;

            return prochaineAnimation;
        }
    }
}
