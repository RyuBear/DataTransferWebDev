using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Transfer.Models.Repository
{
    public class tblCodeMappingRepository : GenericRepository<tblCodeMapping>
    {
        public string DeleteMapping(string SettingName, string Format, string ModeType, string FieldName, string BeforeValue)
        {
            try
            {
                tblCodeMapping mapping = this.Get(x => x.SettingName.Equals(SettingName, StringComparison.OrdinalIgnoreCase)
                                                    && x.Format.Equals(Format, StringComparison.OrdinalIgnoreCase)
                                                    && x.ModeType.Equals(ModeType, StringComparison.OrdinalIgnoreCase)
                                                    && x.FieldName.Equals(FieldName, StringComparison.OrdinalIgnoreCase)
                                                    && x.BeforeValue.Equals(BeforeValue, StringComparison.OrdinalIgnoreCase));
                if (mapping != null)
                {
                    this.Delete(mapping);
                    return "ok";
                }
                else
                    return "刪除時發生錯誤! (原因：設定值不存在)";
            }
            catch (Exception ex)
            {
                return "刪除時發生錯誤! (原因：" + ex.Message.Replace("\r\n", "") + ")";
            }
        }

        /// <summary>
        /// 新增 Code Mapping
        /// </summary>
        /// <returns></returns>
        public string InsertMapping(string SettingName, string Format, string ModeType, string FieldName, string BeforeValue, string AfterValue, string Creator)
        {
            try
            {
                tblCodeMapping mapping = this.Get(x => x.SettingName.Equals(SettingName, StringComparison.OrdinalIgnoreCase)
                                                    && x.Format.Equals(Format, StringComparison.OrdinalIgnoreCase)
                                                    && x.ModeType.Equals(ModeType, StringComparison.OrdinalIgnoreCase)
                                                    && x.FieldName.Equals(FieldName, StringComparison.OrdinalIgnoreCase)
                                                    && x.BeforeValue.Equals(BeforeValue, StringComparison.OrdinalIgnoreCase));
                if (mapping == null)
                {
                    mapping = new tblCodeMapping()
                    {
                        SettingName = SettingName,
                        Format = Format,
                        ModeType = ModeType,
                        FieldName = FieldName,
                        BeforeValue = BeforeValue,
                        AfterValue = AfterValue,
                        CreateTime = Now,
                        Creator = Creator,
                        UpdateTime = Now,
                        Updator = Creator
                    };
                    this.Create(mapping);
                    return "ok";
                }
                else
                    return "新增時發生錯誤! (原因：設定值已存在)";
            }
            catch (Exception ex)
            {
                return "新增時發生錯誤! (原因：" + ex.Message.Replace("\r\n", "") + ")";
            }
        }

        /// <summary>
        /// 更新 Code Mapping
        /// </summary>
        /// <returns></returns>
        public string UpadteMapping(string SettingName, string Format, string ModeType, string FieldName, string BeforeValue, string newBeforeValue, string AfterValue, string Updator)
        {
            try
            {
                tblCodeMapping mapping = this.Get(x => x.SettingName.Equals(SettingName, StringComparison.OrdinalIgnoreCase)
                                                    && x.Format.Equals(Format, StringComparison.OrdinalIgnoreCase)
                                                    && x.ModeType.Equals(ModeType, StringComparison.OrdinalIgnoreCase)
                                                    && x.FieldName.Equals(FieldName, StringComparison.OrdinalIgnoreCase)
                                                    && x.BeforeValue.Equals(BeforeValue, StringComparison.OrdinalIgnoreCase));
                if (mapping != null)
                {
                    tblCodeMapping newOne = new tblCodeMapping()
                    {
                        SettingName = SettingName,
                        Format = Format,
                        ModeType = ModeType,
                        FieldName = FieldName,
                        BeforeValue = newBeforeValue,
                        AfterValue = AfterValue,
                        CreateTime = mapping.CreateTime,
                        Creator = mapping.Creator,
                        UpdateTime = Now,
                        Updator = Updator
                    };
                    this.Delete(mapping);
                    this.Create(newOne);
                    return "ok";
                }
                else
                    return "更新時發生錯誤! (原因：找不到原始設定值)";

            }
            catch (Exception ex)
            {
                return "更新時發生錯誤! (原因：" + ex.Message + ")";
            }
        }
    }
}
