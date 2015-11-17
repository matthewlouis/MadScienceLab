﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Storage;
using System.Collections;

namespace MadScienceLab
{
    public class GameData
    {

        // Save data handling
        StorageDevice device; // HDD saving to
        StorageContainer container; // STFS container to save to
        string containerName = "MadLabGameStorage";
        string filename = "savegame.sav";
        [Serializable]
        public struct SaveGameData
        {
            public int levelCompleted;
            public int currentlevel;
            public int score;
        }

        public SaveGameData saveGameData;

        public GameData()
        {

            // initialize score
            saveGameData.levelCompleted = 0;
            saveGameData.currentlevel = 1;
            saveGameData.score = 0;
            // 
            GetDevice();
            GetContainer();
            if(!Load())
            {
                GetContainer();
                Save();
            }
        }

        public void GetDevice()
        {
            //Starts the selection processes.
            IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            //Sets the global variable.
            device = StorageDevice.EndShowSelector(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();
        }

        public void GetContainer()
        {
            //Starts the selection processes.
            IAsyncResult result = device.BeginOpenContainer(containerName, null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            //Sets the global variable.
            container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();
        }

        public void Save()
        {
            // Check to see whether the save exists.
            if (container.FileExists(filename))
                // Delete it so that we can create one fresh.
                container.DeleteFile(filename);

            using (Stream stream = container.CreateFile(filename))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData)); // create XML serializer object
                serializer.Serialize(stream, saveGameData); // pass saveGameData struct to xml stream
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>bool true if loaded from file. false if no file to load</returns>
        public bool Load()
        {
            // Check to see whether the save exists.
            if (!container.FileExists(filename))
            {
                // If not, dispose of the container and return.
                container.Dispose();
                return false;
            }
            
            // Open the file.
            Stream stream = container.OpenFile(filename, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData)); // create XML serializer object
            saveGameData = (SaveGameData)serializer.Deserialize(stream); // get saved data from stream(file)
            stream.Close();
            container.Dispose();
            return true;
        }
    }
}
