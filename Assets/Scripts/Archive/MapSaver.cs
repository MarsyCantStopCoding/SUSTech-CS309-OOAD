﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DTO;
using GameData;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Units;
using UnityEditor;
using UnityEngine;

namespace Archive
{
    public class MapSaver : MonoBehaviour
    {
        private void Start()
        {
            // Test();
            // TODO: 保存地图功能测试完成之后记得删掉这一部分，MonoBehaviour也删除掉
        }

        public static void Save()
        {
            GameDataManager data = GameDataManager.Instance;
            SaveDTO save = new SaveDTO();
            List<PlayerDTO> players = new List<PlayerDTO>();
            foreach (Player p in data.Players)
            {
                PlayerDTO player = new PlayerDTO(p);
                players.Add(player);
            }

            save.Players = players;
            save.blocks = new List<BlockDTO>();
            
            foreach (KeyValuePair<Vector2, Tuple<GameObject, BlockDTO>> pair in data.blockList)
            {
                save.blocks.Add(pair.Value.Item2);
            }
            save.environment = new List<EnviromentDTO>();
            String json = JsonConvert.SerializeObject(save);

            using (StreamWriter writer = new StreamWriter("save/save_test.json"))
            {
                writer.Write(json);
                writer.Flush();
            }
        }

        public static void SaveAll()
        {
            SaveDTO saveDto = new SaveDTO();
            SaveMapInto(saveDto);
            SavePlayersInto(saveDto);

            string json = saveDto.ToJsonString();
            using StreamWriter writer = new StreamWriter("save/Scene_0.json");
            writer.Write(json);
            writer.Flush();
        }

        /// <summary>
        /// 保存人物，用于地图编辑模式
        /// </summary>
        /// <param name="saveDto"></param>
        public static void SavePlayersInto([NotNull] SaveDTO saveDto)
        {
            GameObject[] allUnits = GameObject.FindGameObjectsWithTag("Unit");
            List<IGrouping<int,GameObject>> groupPlayers = allUnits.GroupBy(unit => unit.GetComponent<Units.Unit>().ofPlayer).ToList();
            List<Player> players = new List<Player>();
            foreach (IGrouping<int,GameObject> unitsOfOnePlayer in groupPlayers)
            {
                Player player = new Player();
                player.UnitsList = unitsOfOnePlayer.Select(unit => unit.GetComponent<Unit>()).ToList();
                player.Index = unitsOfOnePlayer.Key;
                players.Add(player);
            }

            saveDto.Players = players.Select(player => new PlayerDTO(player)).ToList();
        }

        public static void SaveMapInto([NotNull] SaveDTO saveDto)
        {
            GameObject map = GameObject.Find("Map");
            List<GameObject> environments = new List<GameObject>();
            saveDto.blocks ??= new List<BlockDTO>();
            foreach (Transform transform in map.transform)
            {
                GameObject mapElement = transform.gameObject;
                Block block;
                if ((block = mapElement.GetComponent<Block>()) != null)
                {
                    saveDto.blocks.Add(BlockDTO.InitFrom(block));
                }
                else
                {
                    environments.Add(mapElement);
                }
            }

            saveDto.environment ??= new List<EnviromentDTO>();
            List<IGrouping<string,GameObject>> partitionList = environments.
                GroupBy(environment => environment.GetComponent<Environment>().type).ToList();
            foreach (IGrouping<string,GameObject> environmentsInOneGroup in partitionList)
            {
                List<GameObject> certainTypeEnvironments = environmentsInOneGroup.ToList();
                saveDto.environment.Add(EnviromentDTO.InitFrom(environmentsInOneGroup.Key, certainTypeEnvironments));
            }
        }

    }
}