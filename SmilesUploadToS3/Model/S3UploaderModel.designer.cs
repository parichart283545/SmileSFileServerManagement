﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SmilesUploadToS3.Model
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="S3Uploader")]
	public partial class S3UploaderModelDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertUploaderConfig(UploaderConfig instance);
    partial void UpdateUploaderConfig(UploaderConfig instance);
    partial void DeleteUploaderConfig(UploaderConfig instance);
    #endregion
		
		public S3UploaderModelDataContext() : 
				base(global::SmilesUploadToS3.Properties.Settings.Default.S3UploaderConnectionString1, mappingSource)
		{
			OnCreated();
		}
		
		public S3UploaderModelDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public S3UploaderModelDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public S3UploaderModelDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public S3UploaderModelDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<UploaderConfig> UploaderConfigs
		{
			get
			{
				return this.GetTable<UploaderConfig>();
			}
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.usp_DocumentFilelist_Select")]
		public ISingleResult<usp_DocumentFilelist_SelectResult> usp_DocumentFilelist_Select()
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())));
			return ((ISingleResult<usp_DocumentFilelist_SelectResult>)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.usp_UploaderProcess_Update")]
		public ISingleResult<usp_UploaderProcess_UpdateResult> usp_UploaderProcess_Update([global::System.Data.Linq.Mapping.ParameterAttribute(Name="Process", DbType="Bit")] System.Nullable<bool> process)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), process);
			return ((ISingleResult<usp_UploaderProcess_UpdateResult>)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.usp_DocumentFilelistS3_Update")]
		public ISingleResult<usp_DocumentFilelistS3_UpdateResult> usp_DocumentFilelistS3_Update([global::System.Data.Linq.Mapping.ParameterAttribute(Name="Code", DbType="VarChar(20)")] string code, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="S3IsUploaded", DbType="Bit")] System.Nullable<bool> s3IsUploaded, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="S3Bucket", DbType="VarChar(255)")] string s3Bucket, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="S3StorageType", DbType="VarChar(255)")] string s3StorageType, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="S3Key", DbType="VarChar(MAX)")] string s3Key)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), code, s3IsUploaded, s3Bucket, s3StorageType, s3Key);
			return ((ISingleResult<usp_DocumentFilelistS3_UpdateResult>)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.usp_SmileDoclist_Select")]
		public ISingleResult<usp_SmileDoclist_SelectResult> usp_SmileDoclist_Select()
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())));
			return ((ISingleResult<usp_SmileDoclist_SelectResult>)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.usp_SmileDoclistS3_Update")]
		public ISingleResult<usp_SmileDoclistS3_UpdateResult> usp_SmileDoclistS3_Update([global::System.Data.Linq.Mapping.ParameterAttribute(Name="AttachmentID", DbType="VarChar(20)")] string attachmentID, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="S3IsUploaded", DbType="Bit")] System.Nullable<bool> s3IsUploaded, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="S3Bucket", DbType="VarChar(255)")] string s3Bucket, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="S3StorageType", DbType="VarChar(255)")] string s3StorageType, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="S3Key", DbType="VarChar(MAX)")] string s3Key)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), attachmentID, s3IsUploaded, s3Bucket, s3StorageType, s3Key);
			return ((ISingleResult<usp_SmileDoclistS3_UpdateResult>)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.usp_Uploader_Log_Insert")]
		public ISingleResult<usp_Uploader_Log_InsertResult> usp_Uploader_Log_Insert([global::System.Data.Linq.Mapping.ParameterAttribute(Name="Document_ID", DbType="VarChar(20)")] string document_ID, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="FILENAME", DbType="NVarChar(MAX)")] string fILENAME, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="ErrorMessage", DbType="NVarChar(MAX)")] string errorMessage, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="DocType", DbType="NVarChar(MAX)")] string docType)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), document_ID, fILENAME, errorMessage, docType);
			return ((ISingleResult<usp_Uploader_Log_InsertResult>)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.usp_UploaderConfig_Select")]
		public ISingleResult<usp_UploaderConfig_SelectResult> usp_UploaderConfig_Select([global::System.Data.Linq.Mapping.ParameterAttribute(Name="ParameterName", DbType="NVarChar(MAX)")] string parameterName)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), parameterName);
			return ((ISingleResult<usp_UploaderConfig_SelectResult>)(result.ReturnValue));
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.UploaderConfig")]
	public partial class UploaderConfig : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _ParameterName;
		
		private System.Nullable<double> _ValueNumber;
		
		private string _ValueString;
		
		private System.Nullable<System.DateTime> _ValueDateTime;
		
		private System.Nullable<bool> _ValueBoolean;
		
		private string _Remark;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnParameterNameChanging(string value);
    partial void OnParameterNameChanged();
    partial void OnValueNumberChanging(System.Nullable<double> value);
    partial void OnValueNumberChanged();
    partial void OnValueStringChanging(string value);
    partial void OnValueStringChanged();
    partial void OnValueDateTimeChanging(System.Nullable<System.DateTime> value);
    partial void OnValueDateTimeChanged();
    partial void OnValueBooleanChanging(System.Nullable<bool> value);
    partial void OnValueBooleanChanged();
    partial void OnRemarkChanging(string value);
    partial void OnRemarkChanged();
    #endregion
		
		public UploaderConfig()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ParameterName", DbType="NVarChar(255) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string ParameterName
		{
			get
			{
				return this._ParameterName;
			}
			set
			{
				if ((this._ParameterName != value))
				{
					this.OnParameterNameChanging(value);
					this.SendPropertyChanging();
					this._ParameterName = value;
					this.SendPropertyChanged("ParameterName");
					this.OnParameterNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ValueNumber", DbType="Float")]
		public System.Nullable<double> ValueNumber
		{
			get
			{
				return this._ValueNumber;
			}
			set
			{
				if ((this._ValueNumber != value))
				{
					this.OnValueNumberChanging(value);
					this.SendPropertyChanging();
					this._ValueNumber = value;
					this.SendPropertyChanged("ValueNumber");
					this.OnValueNumberChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ValueString", DbType="NVarChar(255)")]
		public string ValueString
		{
			get
			{
				return this._ValueString;
			}
			set
			{
				if ((this._ValueString != value))
				{
					this.OnValueStringChanging(value);
					this.SendPropertyChanging();
					this._ValueString = value;
					this.SendPropertyChanged("ValueString");
					this.OnValueStringChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ValueDateTime", DbType="DateTime")]
		public System.Nullable<System.DateTime> ValueDateTime
		{
			get
			{
				return this._ValueDateTime;
			}
			set
			{
				if ((this._ValueDateTime != value))
				{
					this.OnValueDateTimeChanging(value);
					this.SendPropertyChanging();
					this._ValueDateTime = value;
					this.SendPropertyChanged("ValueDateTime");
					this.OnValueDateTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ValueBoolean", DbType="Bit")]
		public System.Nullable<bool> ValueBoolean
		{
			get
			{
				return this._ValueBoolean;
			}
			set
			{
				if ((this._ValueBoolean != value))
				{
					this.OnValueBooleanChanging(value);
					this.SendPropertyChanging();
					this._ValueBoolean = value;
					this.SendPropertyChanged("ValueBoolean");
					this.OnValueBooleanChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Remark", DbType="NVarChar(MAX)")]
		public string Remark
		{
			get
			{
				return this._Remark;
			}
			set
			{
				if ((this._Remark != value))
				{
					this.OnRemarkChanging(value);
					this.SendPropertyChanging();
					this._Remark = value;
					this.SendPropertyChanged("Remark");
					this.OnRemarkChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	public partial class usp_DocumentFilelist_SelectResult
	{
		
		private string _Code;
		
		private string _FileName;
		
		private string _PhysicalPath;
		
		private string _SubFolder;
		
		public usp_DocumentFilelist_SelectResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Code", DbType="VarChar(20) NOT NULL", CanBeNull=false)]
		public string Code
		{
			get
			{
				return this._Code;
			}
			set
			{
				if ((this._Code != value))
				{
					this._Code = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileName", DbType="NVarChar(MAX)")]
		public string FileName
		{
			get
			{
				return this._FileName;
			}
			set
			{
				if ((this._FileName != value))
				{
					this._FileName = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PhysicalPath", DbType="NVarChar(MAX)")]
		public string PhysicalPath
		{
			get
			{
				return this._PhysicalPath;
			}
			set
			{
				if ((this._PhysicalPath != value))
				{
					this._PhysicalPath = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SubFolder", DbType="NVarChar(MAX)")]
		public string SubFolder
		{
			get
			{
				return this._SubFolder;
			}
			set
			{
				if ((this._SubFolder != value))
				{
					this._SubFolder = value;
				}
			}
		}
	}
	
	public partial class usp_UploaderProcess_UpdateResult
	{
		
		private System.Nullable<bool> _Result;
		
		public usp_UploaderProcess_UpdateResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Result", DbType="Bit")]
		public System.Nullable<bool> Result
		{
			get
			{
				return this._Result;
			}
			set
			{
				if ((this._Result != value))
				{
					this._Result = value;
				}
			}
		}
	}
	
	public partial class usp_DocumentFilelistS3_UpdateResult
	{
		
		private System.Nullable<bool> _Result;
		
		public usp_DocumentFilelistS3_UpdateResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Result", DbType="Bit")]
		public System.Nullable<bool> Result
		{
			get
			{
				return this._Result;
			}
			set
			{
				if ((this._Result != value))
				{
					this._Result = value;
				}
			}
		}
	}
	
	public partial class usp_SmileDoclist_SelectResult
	{
		
		private System.Nullable<int> _AttachmentID;
		
		private string _FileName;
		
		private string _PhysicalPath;
		
		private string _SubFolder;
		
		public usp_SmileDoclist_SelectResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AttachmentID", DbType="Int")]
		public System.Nullable<int> AttachmentID
		{
			get
			{
				return this._AttachmentID;
			}
			set
			{
				if ((this._AttachmentID != value))
				{
					this._AttachmentID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileName", DbType="NVarChar(255)")]
		public string FileName
		{
			get
			{
				return this._FileName;
			}
			set
			{
				if ((this._FileName != value))
				{
					this._FileName = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PhysicalPath", DbType="NVarChar(MAX)")]
		public string PhysicalPath
		{
			get
			{
				return this._PhysicalPath;
			}
			set
			{
				if ((this._PhysicalPath != value))
				{
					this._PhysicalPath = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SubFolder", DbType="NVarChar(85)")]
		public string SubFolder
		{
			get
			{
				return this._SubFolder;
			}
			set
			{
				if ((this._SubFolder != value))
				{
					this._SubFolder = value;
				}
			}
		}
	}
	
	public partial class usp_SmileDoclistS3_UpdateResult
	{
		
		private System.Nullable<bool> _Result;
		
		public usp_SmileDoclistS3_UpdateResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Result", DbType="Bit")]
		public System.Nullable<bool> Result
		{
			get
			{
				return this._Result;
			}
			set
			{
				if ((this._Result != value))
				{
					this._Result = value;
				}
			}
		}
	}
	
	public partial class usp_Uploader_Log_InsertResult
	{
		
		private System.Nullable<bool> _Result;
		
		public usp_Uploader_Log_InsertResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Result", DbType="Bit")]
		public System.Nullable<bool> Result
		{
			get
			{
				return this._Result;
			}
			set
			{
				if ((this._Result != value))
				{
					this._Result = value;
				}
			}
		}
	}
	
	public partial class usp_UploaderConfig_SelectResult
	{
		
		private string _ParameterName;
		
		private System.Nullable<double> _ValueNumber;
		
		private string _ValueString;
		
		private System.Nullable<System.DateTime> _ValueDateTime;
		
		private System.Nullable<bool> _ValueBoolean;
		
		private string _Remark;
		
		public usp_UploaderConfig_SelectResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ParameterName", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string ParameterName
		{
			get
			{
				return this._ParameterName;
			}
			set
			{
				if ((this._ParameterName != value))
				{
					this._ParameterName = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ValueNumber", DbType="Float")]
		public System.Nullable<double> ValueNumber
		{
			get
			{
				return this._ValueNumber;
			}
			set
			{
				if ((this._ValueNumber != value))
				{
					this._ValueNumber = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ValueString", DbType="NVarChar(255)")]
		public string ValueString
		{
			get
			{
				return this._ValueString;
			}
			set
			{
				if ((this._ValueString != value))
				{
					this._ValueString = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ValueDateTime", DbType="DateTime")]
		public System.Nullable<System.DateTime> ValueDateTime
		{
			get
			{
				return this._ValueDateTime;
			}
			set
			{
				if ((this._ValueDateTime != value))
				{
					this._ValueDateTime = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ValueBoolean", DbType="Bit")]
		public System.Nullable<bool> ValueBoolean
		{
			get
			{
				return this._ValueBoolean;
			}
			set
			{
				if ((this._ValueBoolean != value))
				{
					this._ValueBoolean = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Remark", DbType="NVarChar(MAX)")]
		public string Remark
		{
			get
			{
				return this._Remark;
			}
			set
			{
				if ((this._Remark != value))
				{
					this._Remark = value;
				}
			}
		}
	}
}
#pragma warning restore 1591
