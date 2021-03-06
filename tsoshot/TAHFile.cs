using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TAHdecrypt
{
	public class TAHFile : IDisposable
	{
		private string FileName_;
		private TAHHeader Header_;
		private TAHEntrySet EntrySet_;
		private TAHDirectories Files_;
		private object Tag_;
		private Stream Stream_;
		private BinaryReader Reader_;

		public string FileName { get { return FileName_; } set { FileName_ = value; } }
		public TAHHeader Header { get { return Header_; } set { Header_ = value; } }
		public TAHEntrySet EntrySet { get { return EntrySet_; } set { EntrySet_ = value; } }
		public TAHDirectories Files { get { return Files_; } set { Files_ = value; } }
		public object Tag { get { return Tag_; } set { Tag_ = value; } }
		public Stream Stream { get { return Stream_; } set { Stream_ = value; } }
		public BinaryReader Reader { get { return Reader_; } set { Reader_ = value; } }

		public TAHFile(string filename)
		{
			FileName = filename;
			Stream = File.OpenRead(FileName);
			Reader = new BinaryReader(Stream);
		}

		public TAHFile(Stream stream)
		{
			Stream = stream;
			Reader = new BinaryReader(Stream);
		}

		public void Dispose()
		{
			if (Stream != null) {
				Stream.Dispose();
				Stream = null;
			}
		}

		public void LoadEntries()
		{
			Header = TAHHeader.Load(Reader);
			EntrySet = TAHEntrySet.Load(Reader, this);
			Files = TAHDirectories.Load(Reader, this);
		}

		public TAHContent LoadContent(BinaryReader br, TAHEntry e)
		{
			return new TAHContent(e, TAHUtil.ReadEntryData(br, e));
		}
	}

	public class TAHHeader
	{
		public const uint MAGIC = 0x32484154;

		private uint Magic_;
		private int NumEntries_;
		private uint Unknown1_;
		private uint Unknown2_;

		public uint Magic { get { return Magic_; } set { Magic_ = value; } }
		public int NumEntries { get { return NumEntries_; } set { NumEntries_ = value; } }
		public uint Unknown1 { get { return Unknown1_; } set { Unknown1_ = value; } }
		public uint Unknown2 { get { return Unknown2_; } set { Unknown2_ = value; } }

		public static TAHHeader Load(BinaryReader br)
		{
			TAHHeader h = new TAHHeader();
			h.Read(br);
			return h;
		}

		public void Read(BinaryReader br)
		{
			Magic = br.ReadUInt32();
			NumEntries = br.ReadInt32();
			Unknown1 = br.ReadUInt32();
			Unknown2 = br.ReadUInt32();

			if (Magic != TAHHeader.MAGIC)
				throw new Exception("Invalid TAH Magic no");
		}

		public void Write(BinaryWriter bw)
		{
			bw.Write(Magic);
			bw.Write(NumEntries);
			bw.Write(Unknown1);
			bw.Write(Unknown2);
		}
	}

	public class TAHEntrySet
	{
		private List<TAHEntry> Entries_;
		private Dictionary<uint, TAHEntry> EntryMap_;

		public List<TAHEntry> Entries { get { return Entries_; } set { Entries_ = value; } }
		public Dictionary<uint, TAHEntry> EntryMap { get { return EntryMap_; } set { EntryMap_ = value; } }

		public static TAHEntrySet Load(BinaryReader br, TAHFile file)
		{
			TAHEntrySet es = new TAHEntrySet();
			es.Read(br, file);
			return es;
		}

		public void Read(BinaryReader br, TAHFile file)
		{
			Entries = new List<TAHEntry>(file.Header.NumEntries);
			EntryMap = new Dictionary<uint, TAHEntry>();

			for (int i = 0, n = file.Header.NumEntries; i < n; ++i) {
				TAHEntry e = TAHEntry.Load(br, file);

				if (i != 0)
					TailEntry.Length = (int)(e.DataOffset - TailEntry.DataOffset);

				Entries.Add(e);
				EntryMap.Add(e.Hash, e);
			}

			TailEntry.Length = (int)(br.BaseStream.Length - TailEntry.DataOffset);
		}

		public bool TryGetValue(uint hash, out TAHEntry e)
		{
			return EntryMap.TryGetValue(hash, out e);
		}

		public TAHEntry TailEntry { get { return Entries[Count - 1]; } }
		public int Count { get { return Entries.Count; } }
		public TAHEntry this[int index] { get { return Entries[index]; } }
		public TAHEntry this[uint index] { get { return EntryMap[index]; } }
	}

	public class TAHEntry
	{
		private TAHFile Owner_;
		private uint Hash_;
		private uint DataOffset_;
		private int Length_;
		private string FileName_;

		public TAHFile Owner { get { return Owner_; } set { Owner_ = value; } }
		public uint Hash { get { return Hash_; } set { Hash_ = value; } }
		public uint DataOffset { get { return DataOffset_; } set { DataOffset_ = value; } }
		public int Length { get { return Length_; } set { Length_ = value; } }
		public string FileName { get { return FileName_; } set { FileName_ = value; } }

		public static TAHEntry Load(BinaryReader br, TAHFile file)
		{
			TAHEntry h = new TAHEntry();
			h.Read(br, file);
			return h;
		}

		public void Read(BinaryReader br, TAHFile file)
		{
			Hash = br.ReadUInt32();
			DataOffset = br.ReadUInt32();
			Owner = file;
		}

		public void Write(BinaryWriter bw)
		{
			bw.Write(Hash);
			bw.Write(DataOffset);
		}

		public override string ToString()
		{
#if false
			return Hash.ToFormattedString("X", 8, '0')
				+ ": " + DataOffset + ", " + Length
				+ (FileName != null ? FileName : "(unknown)");
#else
			return String.Format("{0:X8} ", Hash) 
				+ ": " + DataOffset + ", " + Length
				+ (FileName != null ? FileName : "(unknown)");
#endif
		}
	}

	public class TAHDirectories
	{
		private List<string> Files_;

		public List<string> Files { get { return Files_; } set { Files_ = value; } }

		public string this[int index]
		{
			get { return Files[index]; }
		}

		public static TAHDirectories Load(BinaryReader br, TAHFile file)
		{
			TAHDirectories dir = new TAHDirectories();
			dir.Read(br, file);
			return dir;
		}

		public void Write(BinaryWriter bw)
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter bw2 = new BinaryWriter(ms);

			foreach (string i in Files) {
				if (i.EndsWith("/"))
					TAHUtil.WriteString(bw2, i);
				else TAHUtil.WriteString(bw2, Path.GetFileName(i));
			}

			bw2.Flush();
			byte[] encrypted = TAHUtil.Encrypt(ms.ToArray());

			bw.Write((uint)ms.Length);
			bw.Write(encrypted);
		}

		public void Read(BinaryReader br, TAHFile file)
		{
			// ディレクトリデータの読み込み
			Files = new List<string>(file.Header.NumEntries);
			int output_length = br.ReadInt32();
			int input_length = (int)(file.EntrySet[0].DataOffset - br.BaseStream.Position);  //- 16 - 8 * file.Header.NumEntries;
			byte[] input = br.ReadBytes(input_length);
			byte[] output = new byte[output_length];

			if (output.Length == 0)
				return;

			TAHUtil.Decrypt(input, output);
			//TAHdecrypt.Decrypter.decrypt(ref input, (uint)input.Length, ref output, (uint)output.Length);

			MemoryStream ms = new MemoryStream(output);
			BinaryReader br2 = new BinaryReader(ms);

			try {
				string dir = "";

				while (ms.Position < ms.Length) {
					string name = TAHUtil.ReadString(br2);

					if (name.Length == 0) {
					}
					else
						if (name.EndsWith("/")) {
							dir = name;
							//DbgPrint("Directory:            " + dir);
						}
						else {
							name = dir + name;
							uint hash = TAHUtil.CalcHash(name);
							TAHEntry ent;

							//DbgPrint(hash.ToString("X").PadLeft(8, '0'));

							if (file.EntrySet.TryGetValue(hash, out ent)) {
								ent.FileName = name;
								//DbgPrint(": Found:     " + file);
							}
							else {
								//DbgPrint(": Not Found: " + file);
								System.Diagnostics.Debug.Assert(false);
							}

							//EntryMap[hash].FileName = FileName;
						}

					Files.Add(name);
				}
			}
			catch (EndOfStreamException) {
			}
		}
	}

	public class TAHContent
	{
		private TAHEntry Entry_;
		private byte[] Data_;

		public TAHEntry Entry { get { return Entry_; } set { Entry_ = value; } }
		public byte[] Data { get { return Data_; } set { Data_ = value; } }

		public TAHContent(TAHEntry e, byte[] data)
		{
			Entry = e;
			Data = data;
		}
	}
}
