using System.Data.Common;

namespace System.Data
{
    public class DbFieldItemReaderStream : Stream
    {
        private DbDataReader? m_Reader;
        private int m_ColumnIndex;
        private long m_Position;


        public DbFieldItemReaderStream(DbDataReader reader, int columnIndex)
        {
            m_Reader = reader;
            m_ColumnIndex = columnIndex;
        }


        public override long Position
        {
            get { return m_Position; }
            set { throw new NotImplementedException(); }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }


        public override int Read(byte[] buffer, int offset, int count)
        {
            if (m_Reader == null)
            {
                throw new InvalidOperationException("Data reader is null.");
            }

            var bytesRead = m_Reader.GetBytes(m_ColumnIndex, m_Position, buffer, offset, count);

            m_Position += bytesRead;

            return (int)bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && null != m_Reader)
            {
                m_Reader.Dispose();
                m_Reader = null;
            }

            base.Dispose(disposing);
        }
    }
}
