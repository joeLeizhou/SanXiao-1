﻿/*******************************************************************************
 * You may amend and distribute as you like, but don't remove this header!
 *
 * EPPlus provides server-side generation of Excel 2007/2010 spreadsheets.
 * See https://github.com/JanKallman/EPPlus for details.
 *
 * Copyright (C) 2011  Jan Källman
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.

 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
 * See the GNU Lesser General Public License for more details.
 *
 * The GNU Lesser General Public License can be viewed at http://www.opensource.org/licenses/lgpl-license.php
 * If you unfamiliar with this license or have questions about it, here is an http://www.gnu.org/licenses/gpl-faq.html
 *
 * All code and executables are provided "as is" with no warranty either express or implied. 
 * The author accepts no liability for any damage or loss of business that this product may cause.
 *
 * Code change notes:
 * 
 * Author							Change						Date
 *******************************************************************************
 * Jan Källman		Added		26-MAR-2012
 *******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using OfficeOpenXml.Utils;
using System.IO;
using OfficeOpenXml.Utils.CompundDocument;

namespace OfficeOpenXml.VBA
{
    /// <summary>
    /// The code signature properties of the project
    /// </summary>
    public class ExcelVbaSignature
    {
        const string schemaRelVbaSignature = "http://schemas.microsoft.com/office/2006/relationships/vbaProjectSignature";
        Packaging.ZipPackagePart _vbaPart = null;
        internal ExcelVbaSignature(Packaging.ZipPackagePart vbaPart)
        {
            _vbaPart = vbaPart;
            GetSignature();
        }
        private void GetSignature()
        {
          
        }
        //Create Oid from a bytearray
        //private string ReadHash(byte[] content)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    int offset = 0x6;
        //    if (0 < (content.Length))
        //    {
        //        byte num = content[offset];
        //        byte num2 = (byte)(num / 40);
        //        builder.Append(num2.ToString(null, null));
        //        builder.Append(".");
        //        num2 = (byte)(num % 40);
        //        builder.Append(num2.ToString(null, null));
        //        ulong num3 = 0L;
        //        for (int i = offset + 1; i < content.Length; i++)
        //        {
        //            num2 = content[i];
        //            num3 = (ulong)(ulong)(num3 << 7) + ((byte)(num2 & 0x7f));
        //            if ((num2 & 0x80) == 0)
        //            {
        //                builder.Append(".");
        //                builder.Append(num3.ToString(null, null));
        //                num3 = 0L;
        //            }
        //            //1.2.840.113549.2.5
        //        }
        //    }


        //    string oId = builder.ToString();

        //    return oId;
        //}
        internal void Save(ExcelVbaProject proj)
        {
            if (Certificate == null)
            {
                return;
            }
            
            if (Certificate.HasPrivateKey==false)    //No signature. Remove any Signature part
            {
                var storeCert = GetCertFromStore(StoreLocation.CurrentUser);
                if (storeCert == null)
                {
                    storeCert = GetCertFromStore(StoreLocation.LocalMachine);
                }
                if (storeCert != null && storeCert.HasPrivateKey == true)
                {
                    Certificate = storeCert;
                }
                else
                {
                    foreach (var r in Part.GetRelationships())
                    {
                        Part.DeleteRelationship(r.Id);
                    }
                    Part.Package.DeletePart(Part.Uri);
                    return;
                }
            }
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            byte[] certStore = GetCertStore();

            byte[] cert = SignProject(proj);
            bw.Write((uint)cert.Length);
            bw.Write((uint)44);                  //?? 36 ref inside cert ??
            bw.Write((uint)certStore.Length);    //cbSigningCertStore
            bw.Write((uint)(cert.Length + 44));  //certStoreOffset
            bw.Write((uint)0);                   //cbProjectName
            bw.Write((uint)(cert.Length + certStore.Length + 44));    //projectNameOffset
            bw.Write((uint)0);    //fTimestamp
            bw.Write((uint)0);    //cbTimestampUrl
            bw.Write((uint)(cert.Length + certStore.Length + 44 + 2));    //timestampUrlOffset
            bw.Write(cert);
            bw.Write(certStore);
            bw.Write((ushort)0);//rgchProjectNameBuffer
            bw.Write((ushort)0);//rgchTimestampBuffer
            bw.Write((ushort)0);
            bw.Flush();

            var rel = proj.Part.GetRelationshipsByType(schemaRelVbaSignature).FirstOrDefault();
            if (Part == null)
            {

                if (rel != null)
                {
                    Uri = rel.TargetUri;
                    Part = proj._pck.GetPart(rel.TargetUri);
                }
                else
                {
                    Uri = new Uri("/xl/vbaProjectSignature.bin", UriKind.Relative);
                    Part = proj._pck.CreatePart(Uri, ExcelPackage.schemaVBASignature);
                }
            }
            if (rel == null)
            {
                proj.Part.CreateRelationship(UriHelper.ResolvePartUri(proj.Uri, Uri), Packaging.TargetMode.Internal, schemaRelVbaSignature);                
            }
            var b = ms.ToArray();
            Part.GetStream(FileMode.Create).Write(b, 0, b.Length);            
        }

        private X509Certificate2 GetCertFromStore(StoreLocation loc)
        {
            try
            {
                X509Store store = new X509Store(StoreName.My, loc);
                store.Open(OpenFlags.ReadOnly);
                try
                {
                    var storeCert = store.Certificates.Find(
                                    X509FindType.FindByThumbprint,
                                    Certificate.Thumbprint,
                                    true
                                    ).OfType<X509Certificate2>().FirstOrDefault();
                    return storeCert;
                }
                finally
                {
                    #if Core
                        store.Dispose();
                    #endif
                    store.Close();
                }
            }
            catch
            {
                return null;
            }
        }

        private byte[] GetCertStore()
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            bw.Write((uint)0); //Version
            bw.Write((uint)0x54524543); //fileType

            //SerializedCertificateEntry
            var certData = Certificate.RawData;
            bw.Write((uint)0x20);
            bw.Write((uint)1);
            bw.Write((uint)certData.Length);
            bw.Write(certData);

            //EndElementMarkerEntry
            bw.Write((uint)0);
            bw.Write((ulong)0);

            bw.Flush();
            return ms.ToArray();
        }

        private void WriteProp(BinaryWriter bw, int id, byte[] data)
        {
            bw.Write((uint)id);
            bw.Write((uint)1);
            bw.Write((uint)data.Length);
            bw.Write(data);
        }
        internal byte[] SignProject(ExcelVbaProject proj)
        {
            return new byte[0];
        }

        private byte[] GetContentHash(ExcelVbaProject proj)
        {
            //MS-OVBA 2.4.2
            var enc = System.Text.Encoding.GetEncoding(proj.CodePage);
            BinaryWriter bw = new BinaryWriter(new MemoryStream());
            bw.Write(enc.GetBytes(proj.Name));
            bw.Write(enc.GetBytes(proj.Constants));
            foreach (var reference in proj.References)
            {
                if (reference.ReferenceRecordID == 0x0D)
                {
                    bw.Write((byte)0x7B);
                }
                if (reference.ReferenceRecordID == 0x0E)
                {
                    //var r = (ExcelVbaReferenceProject)reference;
                    //BinaryWriter bwTemp = new BinaryWriter(new MemoryStream());
                    //bwTemp.Write((uint)r.Libid.Length);
                    //bwTemp.Write(enc.GetBytes(r.Libid));              
                    //bwTemp.Write((uint)r.LibIdRelative.Length);
                    //bwTemp.Write(enc.GetBytes(r.LibIdRelative));
                    //bwTemp.Write(r.MajorVersion);
                    //bwTemp.Write(r.MinorVersion);
                    foreach (byte b in BitConverter.GetBytes((uint)reference.Libid.Length))  //Length will never be an UInt with 4 bytes that aren't 0 (> 0x00FFFFFF), so no need for the rest of the properties.
                    {
                        if (b != 0)
                        {
                            bw.Write(b);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            foreach (var module in proj.Modules)
            {
                var lines = module.Code.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (!line.StartsWith("attribute", StringComparison.OrdinalIgnoreCase))
                    {
                        bw.Write(enc.GetBytes(line));
                    }
                }
            }
            var buffer = (bw.BaseStream as MemoryStream).ToArray();
            var hp = System.Security.Cryptography.MD5.Create();
            return hp.ComputeHash(buffer);
        }
        /// <summary>
        /// The certificate to sign the VBA project.
        /// <remarks>
        /// This certificate must have a private key.
        /// There is no validation that the certificate is valid for codesigning, so make sure it's valid to sign Excel files (Excel 2010 is more strict that prior versions).
        /// </remarks>
        /// </summary>
        public X509Certificate2 Certificate { get; set; }
        /// <summary>
        /// The verifier
        /// </summary>
        internal CompoundDocument Signature { get; set; }
        internal Packaging.ZipPackagePart Part { get; set; }
        internal Uri Uri { get; private set; }
    }
}