// Programed by N765/Konoa

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

// TAHdecryptorがあそこまで複雑なのは理解し難いが

namespace TDCGExplorer
{
    public class TAHWriter
    {
        private TAHHeader header;
        private List<TAHContent> contents;
        private TAHDirectories directory;
        private List<string> delegateFileList;

        // ファイル取得delegate
        public delegate Byte[] DataHandler(string filename);
        public DataHandler Data;

        public TAHWriter()
        {
            header = new TAHHeader();
            contents = new List<TAHContent>();
            directory = new TAHDirectories();
            header.Version = 1;
            delegateFileList = new List<string>();
        }

        // TAHにファイルを加える.
        public void Add(string filename)
        {
            // オリジナルのファイル名を保存しておく.
            delegateFileList.Add(filename);
            // TAHに格納可能なファイル名に変換する.
            string regularfile = filename.Replace('\\', '/');
            TAHEntry tahentry = new TAHEntry();
            tahentry.DataOffset = 0;
            tahentry.FileName = regularfile;
            tahentry.Length = 0;
            if (Path.GetDirectoryName(regularfile) == "")
            {
                // dddddddd_xxxxxxxx.eee形式をハッシュ値に戻す
                if (regularfile.Length >= 17)
                {
                    string hashcode = regularfile.Substring(9, 8);
                    try
                    {
                        // 16進数からハッシュ値に.
                        tahentry.Hash = UInt32.Parse(hashcode, System.Globalization.NumberStyles.HexNumber);
                        // 暫定ファイル名は削除.
                        tahentry.FileName = null;
                    }
                    catch (Exception)
                    {
                        // 判んない時は適当につける.
                        tahentry.Hash = TAHUtil.CalcHash(regularfile);
                    }
                }
            }
            else
            {
                tahentry.Hash = TAHUtil.CalcHash(regularfile);
            }
            TAHContent content = new TAHContent(tahentry,null);
            contents.Add(content);
        }

        public int Count
        {
            get { return contents.Count; }
        }

        // ヘッダーを構築する.
        private void BuildHeader()
        {
            // ヘッダー値を設定する. versionは外部アクセサから設定する.
            header.Magic = 0x32484154; // TAH2
            header.NumEntries = contents.Count;
            header.Unknown2 = 0;
        }

        // ファイル一覧を作成する.
        private void BuildDirectory()
        {
            Dictionary<string, int> pathfragment = new Dictionary<string, int>();
            List<string> files = new List<string>();
            foreach (TAHContent content in contents)
                if (content.Entry.FileName != null) files.Add(content.Entry.FileName);
            files.Sort();
            // ディレクトリ情報を構築する.
            foreach (string filename in files)
            {
                string[] pathelement = filename.Split('/');
                string fullpath = "";
                for (int index = 0; index < (pathelement.Length - 1); index++)
                {
                    fullpath += pathelement[index] + "/";
                    if (pathfragment.ContainsKey(fullpath.ToLower()) == false)
                    {
                        pathfragment.Add(fullpath.ToLower(), 1);
                        directory.Files.Add(fullpath);
                        // 大文字小文字は無視する
                        foreach (string subfilename in files)
                        {
                            // その階層の下にあるファイルは全て書き出す.
                            string[] subpathelement = subfilename.Split('/');
                            string subpath = "";
                            for (int subindex = 0; subindex < (subpathelement.Length - 1); subindex++)
                                subpath += subpathelement[subindex] + "/";
                            if (subpath.ToLower() == fullpath.ToLower())
                            {
                                directory.Files.Add(subpathelement[subpathelement.Length - 1]);
                            }
                        }
                    }
                }
            }
        }

        // TAHバージョンのアクセサ
        public uint Version
        {
            get { return header.Version; }
            set { header.Version = value; }
        }

        // TAHファイルをstreamに書き込む.成功ならtrue,失敗ならfalseを返す.
        public bool Write(Stream tostream)
        {
            try
            {
                int index = 0;
                // ファイル先頭に移動する.
                tostream.Seek(0, SeekOrigin.Begin);
                using (BufferedStream stream = new BufferedStream(tostream))
                using (BinaryWriter binarywriter = new BinaryWriter(stream))
                {
                    // ヘッダー情報を構築する.
                    BuildHeader();
                    // ヘッダーを書き込む.
                    header.Write(binarywriter);
                    long entryposition = stream.Position;
                    // offset値は0のままでエントリを一度書き込む.
                    foreach (TAHContent content in contents)
                        content.Entry.Write(binarywriter);
                    // ディレクトリを構築する
                    BuildDirectory();
                    // ディレクトリを書き込む.
                    directory.Write(binarywriter);
                    // データを書き込む
                    foreach (TAHContent content in contents)
                    {
                        // 実際にデータを読み込む.
                        Byte[] data = Data(delegateFileList[index++]);
                        // ヘッダーを更新する.
                        content.Data = data;
                        content.Entry.Length = data.Length;
                        content.Entry.DataOffset = (uint)stream.Position;
                        UInt32 length = (UInt32)content.Entry.Length;
                        binarywriter.Write(length);
                        Byte[] cryptdata = TAHUtil.Encrypt(content.Data);
                        binarywriter.Write(cryptdata);
                        // 参照を解除する.
                        content.Data = null;
                        data = null;
                    }
                    // エントリを書き直す.
                    stream.Seek(entryposition, SeekOrigin.Begin);
                    foreach (TAHContent content in contents)
                        content.Entry.Write(binarywriter);
                    // バッファをフラッシュして完了.
                    binarywriter.Flush();
                    binarywriter.Close();
                    //stream.Flush();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
    }
}

/*

 * 
 * Analyzed by N765/Konoa.
 * 
 
     TAHの構造

    uint32 magic;      // TAH2
    uint32 numentries; // TAHEntryの数
    uint32 version;    // TAHバージョン
    uint32 nouse;      // 予約

    loop(numentries){
      uint32 hash;     // ハッシュ値
      uint32 offset;   // データ先頭オフセット
    }

    uint32 dirlen;     // 展開後のディレクトリテーブル長

    lzss(inputlen=entries[0].offset-ftell(fp)){ // 現在のファイル位置から最初のentriesのoffsetまで
    <lzss+mtcrypt data>
    example:
	    data/               // ディレクトリの断片はあまり重要ではないらしい
	    script/
	    script/items/       // 最後に示されたディレクトリ名がファイル名に結合する
	    N765BODY_A00.TBN    // 大文字小文字は関係ない。内部的には大文字にする方が間違い。
	    data/icon/
	    data/model/
	    N765BODY_A00.TSO
	    data/icon/items/
	    N765BODY_A00.PSD
    }
    entries[0].offset:		// データ長は次のoffsetまで
    <lzss+mtcrypt data>
    例えばN765BODY_A00.TBN
    entries[1].offset:
    <lzss+mtcrypt data>
    例えばN765BODY_A00.TSO
    entries[1].offset:
    <lzss+mtcrypt data>
    例えばN765BODY_A00.PSD		// データ長はEOFまで
    <EOF>

 */
