﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SmilesUploadToS3.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class S3UploaderDatacontext : DbContext
    {
        public S3UploaderDatacontext()
            : base("name=S3UploaderDatacontext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<UploaderConfig> UploaderConfig { get; set; }
    
        public virtual ObjectResult<usp_DocumentFilelist_Select_Result> usp_DocumentFilelist_Select()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_DocumentFilelist_Select_Result>("usp_DocumentFilelist_Select");
        }
    
        public virtual ObjectResult<Nullable<bool>> usp_DocumentFilelistS3_Update(string code, Nullable<bool> s3IsUploaded, string s3Bucket, string s3StorageType, string s3Key)
        {
            var codeParameter = code != null ?
                new ObjectParameter("Code", code) :
                new ObjectParameter("Code", typeof(string));
    
            var s3IsUploadedParameter = s3IsUploaded.HasValue ?
                new ObjectParameter("S3IsUploaded", s3IsUploaded) :
                new ObjectParameter("S3IsUploaded", typeof(bool));
    
            var s3BucketParameter = s3Bucket != null ?
                new ObjectParameter("S3Bucket", s3Bucket) :
                new ObjectParameter("S3Bucket", typeof(string));
    
            var s3StorageTypeParameter = s3StorageType != null ?
                new ObjectParameter("S3StorageType", s3StorageType) :
                new ObjectParameter("S3StorageType", typeof(string));
    
            var s3KeyParameter = s3Key != null ?
                new ObjectParameter("S3Key", s3Key) :
                new ObjectParameter("S3Key", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<bool>>("usp_DocumentFilelistS3_Update", codeParameter, s3IsUploadedParameter, s3BucketParameter, s3StorageTypeParameter, s3KeyParameter);
        }
    
        public virtual ObjectResult<usp_SmileDoclist_Select_Result> usp_SmileDoclist_Select()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_SmileDoclist_Select_Result>("usp_SmileDoclist_Select");
        }
    
        public virtual ObjectResult<Nullable<bool>> usp_SmileDoclistS3_Update(string attachmentID, Nullable<bool> s3IsUploaded, string s3Bucket, string s3StorageType, string s3Key)
        {
            var attachmentIDParameter = attachmentID != null ?
                new ObjectParameter("AttachmentID", attachmentID) :
                new ObjectParameter("AttachmentID", typeof(string));
    
            var s3IsUploadedParameter = s3IsUploaded.HasValue ?
                new ObjectParameter("S3IsUploaded", s3IsUploaded) :
                new ObjectParameter("S3IsUploaded", typeof(bool));
    
            var s3BucketParameter = s3Bucket != null ?
                new ObjectParameter("S3Bucket", s3Bucket) :
                new ObjectParameter("S3Bucket", typeof(string));
    
            var s3StorageTypeParameter = s3StorageType != null ?
                new ObjectParameter("S3StorageType", s3StorageType) :
                new ObjectParameter("S3StorageType", typeof(string));
    
            var s3KeyParameter = s3Key != null ?
                new ObjectParameter("S3Key", s3Key) :
                new ObjectParameter("S3Key", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<bool>>("usp_SmileDoclistS3_Update", attachmentIDParameter, s3IsUploadedParameter, s3BucketParameter, s3StorageTypeParameter, s3KeyParameter);
        }
    
        public virtual ObjectResult<Nullable<bool>> usp_Uploader_Log_Insert(string document_ID, string fILENAME, string errorMessage, string docType)
        {
            var document_IDParameter = document_ID != null ?
                new ObjectParameter("Document_ID", document_ID) :
                new ObjectParameter("Document_ID", typeof(string));
    
            var fILENAMEParameter = fILENAME != null ?
                new ObjectParameter("FILENAME", fILENAME) :
                new ObjectParameter("FILENAME", typeof(string));
    
            var errorMessageParameter = errorMessage != null ?
                new ObjectParameter("ErrorMessage", errorMessage) :
                new ObjectParameter("ErrorMessage", typeof(string));
    
            var docTypeParameter = docType != null ?
                new ObjectParameter("DocType", docType) :
                new ObjectParameter("DocType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<bool>>("usp_Uploader_Log_Insert", document_IDParameter, fILENAMEParameter, errorMessageParameter, docTypeParameter);
        }
    
        public virtual ObjectResult<usp_UploaderConfig_Select_Result> usp_UploaderConfig_Select(string parameterName)
        {
            var parameterNameParameter = parameterName != null ?
                new ObjectParameter("ParameterName", parameterName) :
                new ObjectParameter("ParameterName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_UploaderConfig_Select_Result>("usp_UploaderConfig_Select", parameterNameParameter);
        }
    }
}
