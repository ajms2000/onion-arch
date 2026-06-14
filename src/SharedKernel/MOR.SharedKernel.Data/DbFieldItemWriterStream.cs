using System.Data.Common;

namespace System.Data
{
    internal class DbFieldItemWriterStream : Stream
    {
        public DbCommand? InsertCommand { get; set; }
        public DbParameter? InsertDataParam { get; set; }
        public DbCommand? UpdateCommand { get; set; }
        public DbParameter? UpdateDataParam { get; set; }


        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position { get; set; }


        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
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
            var data = buffer;

            if (offset != 0 || count != buffer.Length)
            {
                data = new byte[count];
                Array.Copy(buffer, offset, data, 0, count);
            }

            if (InsertCommand != null && InsertDataParam != null && Position == 0)
            {
                InsertDataParam.Value = data;
                InsertCommand.ExecuteNonQuery();
            }
            else if (UpdateCommand != null && UpdateDataParam != null)
            {
                UpdateDataParam.Value = data;
                UpdateCommand.ExecuteNonQuery();
            }
            else
            {
                throw new InvalidOperationException("Command/Params are null.");
            }

            Position += count;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (InsertCommand != null)
                {
                    InsertCommand.Dispose();
                    InsertCommand = null;
                }

                if (UpdateCommand != null)
                {
                    UpdateCommand.Dispose();
                    UpdateCommand = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
