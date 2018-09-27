﻿using CognitivePlayground.Model.Interface;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using config = System.Configuration;

namespace CognitivePlayground.Model
{
    public class ConfigurationManager : IConfigurationManager
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ConfigurationManager));
        private static string _actionsKey = "actions";
        private static string _azureSubscriptionKey = "subscriptionKey";
        private static string _videoStreamAddressKey = "streamAddress";
        private static string _azureFaceApiUri = "faceApiUri";
        

        public IEnumerable<Action> GetActions()
        {
            try
            {
                _logger.Info("GetActions() called");
                var actionsJson = GetAppSettings(_actionsKey);
                var actions = JsonConvert.DeserializeObject<List<Action>>(actionsJson);
                return actions;
            }
            catch (Exception ex)
            {
                _logger.Error($"GetActions throw an exception {ex.Message} {ex.StackTrace}");
            }
            return null;
        }

        public string GetAzureApiUri()
        {
            try
            {
                _logger.Info("GetAzureApiUri() called");
                var azureFaceApiUri = GetAppSettings(_azureFaceApiUri);
                return azureFaceApiUri;
            }
            catch (Exception ex)
            {
                _logger.Error($"GetAzureApiUri throw an exception {ex.Message} {ex.StackTrace}");
            }
            return null;
        }

        public string GetAzureSubscriptionKey()
        {
            try
            {
                _logger.Info("GetAzureSubscriptionKey() called");
                var subscriptionKey = GetAppSettings(_azureSubscriptionKey);
                return subscriptionKey;
            }
            catch (Exception ex)
            {
                _logger.Error($"GetAzureSubscriptionKey throw an exception {ex.Message} {ex.StackTrace}");
            }
            return null;
        }

        public string GetVideoStreamAddress()
        {
            try
            {
                _logger.Info("GetVideoStreamAddress() called");
                var streamAddress = GetAppSettings(_videoStreamAddressKey);
                return streamAddress;
            }
            catch (Exception ex)
            {
                _logger.Error($"GetVideoStreamAddress throw an exception {ex.Message} {ex.StackTrace}");
            }
            return null;
        }

        public void SaveActions(IEnumerable<Action> actions)
        {
            try
            {
                _logger.Info("SaveActions() called");
                var json = JsonConvert.SerializeObject(actions);
                _logger.Info($"SaveActions() json to be used {json}");
                AddOrUpdateAppSettings(_actionsKey, json);
            }
            catch (Exception ex)
            {
                _logger.Error($"SaveActions throw an exception {ex.Message} {ex.StackTrace}");
            }
        }

        public void SaveVideoStreamAddress(string address)
        {
            try
            {
                _logger.Info($"SaveVideoStreamAddress(\"{address}\") called");
                AddOrUpdateAppSettings(_videoStreamAddressKey, address);
            }
            catch (Exception ex)
            {
                _logger.Error($"SaveVideoStreamAddress throw an exception {ex.Message} {ex.StackTrace}");
            }
        }

        private void AddOrUpdateAppSettings(string key, string value)
        {
            try
            {
                _logger.Info($"AddOrUpdateAppSettings(\"{key}\", \"{value}\") called");
                var configFile = config.ConfigurationManager.OpenExeConfiguration(config.ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(config.ConfigurationSaveMode.Modified);
                config.ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (config.ConfigurationErrorsException ex)
            {
                _logger.Error($"AddOrUpdateAppSettings throw an exception {ex.Message} {ex.StackTrace}");
            }
            catch (Exception ex)
            {
                _logger.Error($"AddOrUpdateAppSettings throw an exception {ex.Message} {ex.StackTrace}");
            }
        }

        private string GetAppSettings(string key)
        {
            try
            {
                _logger.Info($"GetAppSettings(\"{key}\") called");
                var setting = config.ConfigurationManager.AppSettings.Get(key);
                return setting;
            }
            catch (Exception ex)
            {
                _logger.Error($"GetAppSettings throw an exception {ex.Message} {ex.StackTrace}");
            }
            return null;
        }
    }
}