HES: Hazardous Extraction Systems

A rewrite of H.A.R.V.E.S.T.E.R, redoing core game mechanics, graphics, and architecture with the lessons learned from H.A.R.V.E.S.T.E.R

Developed with Unity 6.6.9f1.

To install this project, fork and clone it into a preferred directory. Then open Unity Hub and press "Add" -> "Add from disk". Then select the cloned directory.
Git LFS is required for installation.

This project currently uses Unity's Cinemachine 3, TextMeshPro, and FMOD.

Architecture Briefing:
All "data" (names, stats, settings, FMOD events, etc.) are held within Unity's ScriptableObjects.
ScriptableObject scripts are found in the assets/scripts/scriptableobjects folder. Their corresponding assets are found in the assets/data folder.
Globals are used to share real-time data (input, velocities, etc.) with the rest of the game.
Every effort is made to ensure that Audio, Visual, and Behavioral scripts are separated.
Audiovisual scripts depend on behavioral scripts through events. Audio and visuals are not co-dependent.

Borrowed Assets:
Skybox: alexandr-melas on cgtrader. Royalty-free license. Available for use in commercial works. 
Viper/Hornet Font: 0x408 on GitHub. MIT license.

Programmer: Adham Alghool , Adhams.Alghool@gmail.com
3D Modeler / Texturer / Collaborator: Hans Vondrell , hansvondrell@gmail.com
Music Producer: Logan Myer
