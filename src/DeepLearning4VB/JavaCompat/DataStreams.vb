Imports System
Imports System.IO
Imports System.Text

Namespace JavaCompat

    ''' <summary>
    ''' Binary input with the semantics of <c>java.io.DataInput</c>: big-endian multi-byte values and
    ''' "modified UTF-8" strings written by <see cref="DataOutput.writeUTF"/>. Byte arrays are
    ''' <c>Byte()</c>; <see cref="readByte"/> returns the signed value like Java.
    ''' </summary>
    Public Interface DataInput
        Sub readFully(ByVal b() As Byte)
        Sub readFully(ByVal b() As Byte, ByVal off As Integer, ByVal len As Integer)
        Function skipBytes(ByVal n As Integer) As Integer
        Function readBoolean() As Boolean
        Function readByte() As SByte
        Function readUnsignedByte() As Integer
        Function readShort() As Short
        Function readUnsignedShort() As Integer
        Function readChar() As Char
        Function readInt() As Integer
        Function readLong() As Long
        Function readFloat() As Single
        Function readDouble() As Double
        Function readLine() As String
        Function readUTF() As String
    End Interface

    ''' <summary>Binary output with the semantics of <c>java.io.DataOutput</c> (big-endian, modified UTF-8).</summary>
    Public Interface DataOutput
        Sub write(ByVal b As Integer)
        Sub write(ByVal b() As Byte)
        Sub write(ByVal b() As Byte, ByVal off As Integer, ByVal len As Integer)
        Sub writeBoolean(ByVal v As Boolean)
        Sub writeByte(ByVal v As Integer)
        Sub writeShort(ByVal v As Integer)
        Sub writeChar(ByVal v As Integer)
        Sub writeInt(ByVal v As Integer)
        Sub writeLong(ByVal v As Long)
        Sub writeFloat(ByVal v As Single)
        Sub writeDouble(ByVal v As Double)
        Sub writeBytes(ByVal s As String)
        Sub writeChars(ByVal s As String)
        Sub writeUTF(ByVal s As String)
    End Interface

    ''' <summary>A <see cref="DataInput"/> over a <see cref="Stream"/> (<c>java.io.DataInputStream</c>).</summary>
    Public Class DataInputStream
        Implements DataInput, IDisposable

        Private ReadOnly stream As Stream
        Private ReadOnly leaveOpen As Boolean
        Private ReadOnly scratch(7) As Byte

        Public Sub New(ByVal stream As Stream)
            Me.New(stream, False)
        End Sub

        Public Sub New(ByVal stream As Stream, ByVal leaveOpen As Boolean)
            If stream Is Nothing Then Throw New ArgumentNullException(NameOf(stream))
            Me.stream = stream
            Me.leaveOpen = leaveOpen
        End Sub

        Public ReadOnly Property BaseStream As Stream
            Get
                Return stream
            End Get
        End Property

        ''' <summary>Reads up to <paramref name="len"/> bytes; returns the number read, or -1 at end of stream (Java <c>read</c>).</summary>
        Public Function read(ByVal b() As Byte, ByVal off As Integer, ByVal len As Integer) As Integer
            Dim n As Integer = stream.Read(b, off, len)
            Return If(n = 0 AndAlso len > 0, -1, n)
        End Function

        Public Function read(ByVal b() As Byte) As Integer
            Return read(b, 0, b.Length)
        End Function

        ''' <summary>Reads one unsigned byte, or -1 at end of stream (Java <c>read()</c>).</summary>
        Public Function read() As Integer
            Return stream.ReadByte()
        End Function

        Public Sub readFully(ByVal b() As Byte) Implements DataInput.readFully
            readFully(b, 0, b.Length)
        End Sub

        Public Sub readFully(ByVal b() As Byte, ByVal off As Integer, ByVal len As Integer) Implements DataInput.readFully
            Dim total As Integer = 0
            While total < len
                Dim n As Integer = stream.Read(b, off + total, len - total)
                If n <= 0 Then Throw New EndOfStreamException()
                total += n
            End While
        End Sub

        Public Function skipBytes(ByVal n As Integer) As Integer Implements DataInput.skipBytes
            Dim skipped As Integer = 0
            If stream.CanSeek Then
                Dim remaining As Long = stream.Length - stream.Position
                skipped = CInt(Math.Min(n, Math.Max(0, remaining)))
                stream.Seek(skipped, SeekOrigin.Current)
                Return skipped
            End If
            While skipped < n
                If stream.ReadByte() < 0 Then Exit While
                skipped += 1
            End While
            Return skipped
        End Function

        Public Function readBoolean() As Boolean Implements DataInput.readBoolean
            Return readUnsignedByte() <> 0
        End Function

        Public Function readByte() As SByte Implements DataInput.readByte
            Return CSByte(CType(readUnsignedByte(), Byte))
        End Function

        Public Function readUnsignedByte() As Integer Implements DataInput.readUnsignedByte
            Dim v As Integer = stream.ReadByte()
            If v < 0 Then Throw New EndOfStreamException()
            Return v
        End Function

        Public Function readShort() As Short Implements DataInput.readShort
            readFully(scratch, 0, 2)
            ' widen before shifting: a Byte << 8 stays a Byte in VB (the shift count is masked to 3 bits)
            Return CShort((CInt(scratch(0)) << 8) Or CInt(scratch(1)))
        End Function

        Public Function readUnsignedShort() As Integer Implements DataInput.readUnsignedShort
            readFully(scratch, 0, 2)
            Return (CInt(scratch(0)) << 8) Or CInt(scratch(1))
        End Function

        Public Function readChar() As Char Implements DataInput.readChar
            Return ChrW(readUnsignedShort())
        End Function

        Public Function readInt() As Integer Implements DataInput.readInt
            readFully(scratch, 0, 4)
            Return (CInt(scratch(0)) << 24) Or (CInt(scratch(1)) << 16) Or (CInt(scratch(2)) << 8) Or CInt(scratch(3))
        End Function

        Public Function readLong() As Long Implements DataInput.readLong
            readFully(scratch, 0, 8)
            Dim v As Long = 0
            For i As Integer = 0 To 7
                v = (v << 8) Or scratch(i)
            Next i
            Return v
        End Function

        Public Function readFloat() As Single Implements DataInput.readFloat
            Dim bits As Integer = readInt()
            Return BitConverter.ToSingle(BitConverter.GetBytes(bits), 0)
        End Function

        Public Function readDouble() As Double Implements DataInput.readDouble
            Return BitConverter.Int64BitsToDouble(readLong())
        End Function

        ''' <summary>Reads a line of Latin-1 text terminated by LF, CR or CRLF; Nothing at end of stream (deprecated in Java, kept for completeness).</summary>
        Public Function readLine() As String Implements DataInput.readLine
            Dim sb As New StringBuilder()
            Dim c As Integer = stream.ReadByte()
            If c < 0 Then Return Nothing
            While c >= 0
                If c = 10 Then Exit While
                If c = 13 Then
                    If stream.CanSeek Then
                        Dim nextByte As Integer = stream.ReadByte()
                        If nextByte >= 0 AndAlso nextByte <> 10 Then stream.Seek(-1, SeekOrigin.Current)
                    End If
                    Exit While
                End If
                sb.Append(ChrW(c))
                c = stream.ReadByte()
            End While
            Return sb.ToString()
        End Function

        ''' <summary>Reads a string written by <see cref="DataOutputStream.writeUTF"/> (2-byte length prefix, modified UTF-8).</summary>
        Public Function readUTF() As String Implements DataInput.readUTF
            Dim utfLen As Integer = readUnsignedShort()
            Dim bytes(utfLen - 1) As Byte
            readFully(bytes)
            Return DecodeModifiedUtf8(bytes)
        End Function

        Friend Shared Function DecodeModifiedUtf8(ByVal bytes() As Byte) As String
            Dim sb As New StringBuilder(bytes.Length)
            Dim i As Integer = 0
            While i < bytes.Length
                Dim c As Integer = bytes(i)
                If c < &H80 Then
                    sb.Append(ChrW(c))
                    i += 1
                ElseIf (c And &HE0) = &HC0 Then
                    If i + 1 >= bytes.Length Then Throw New FormatException("malformed input: partial character at end")
                    sb.Append(ChrW(((c And &H1F) << 6) Or (bytes(i + 1) And &H3F)))
                    i += 2
                ElseIf (c And &HF0) = &HE0 Then
                    If i + 2 >= bytes.Length Then Throw New FormatException("malformed input: partial character at end")
                    sb.Append(ChrW(((c And &HF) << 12) Or ((bytes(i + 1) And &H3F) << 6) Or (bytes(i + 2) And &H3F)))
                    i += 3
                Else
                    Throw New FormatException("malformed input around byte " & i)
                End If
            End While
            Return sb.ToString()
        End Function

        Public Sub Close()
            Dispose()
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            If Not leaveOpen Then stream.Dispose()
        End Sub
    End Class

    ''' <summary>A <see cref="DataOutput"/> over a <see cref="Stream"/> (<c>java.io.DataOutputStream</c>).</summary>
    Public Class DataOutputStream
        Implements DataOutput, IDisposable

        Private ReadOnly stream As Stream
        Private ReadOnly leaveOpen As Boolean
        Private ReadOnly scratch(7) As Byte
        Private written As Long

        Public Sub New(ByVal stream As Stream)
            Me.New(stream, False)
        End Sub

        Public Sub New(ByVal stream As Stream, ByVal leaveOpen As Boolean)
            If stream Is Nothing Then Throw New ArgumentNullException(NameOf(stream))
            Me.stream = stream
            Me.leaveOpen = leaveOpen
        End Sub

        Public ReadOnly Property BaseStream As Stream
            Get
                Return stream
            End Get
        End Property

        ''' <summary>The number of bytes written so far (Java <c>size()</c>).</summary>
        Public Function size() As Integer
            Return CInt(Math.Min(written, Integer.MaxValue))
        End Function

        Public Sub write(ByVal b As Integer) Implements DataOutput.write
            stream.WriteByte(CByte(b And &HFF))
            written += 1
        End Sub

        Public Sub write(ByVal b() As Byte) Implements DataOutput.write
            write(b, 0, b.Length)
        End Sub

        Public Sub write(ByVal b() As Byte, ByVal off As Integer, ByVal len As Integer) Implements DataOutput.write
            stream.Write(b, off, len)
            written += len
        End Sub

        Public Sub writeBoolean(ByVal v As Boolean) Implements DataOutput.writeBoolean
            write(If(v, 1, 0))
        End Sub

        Public Sub writeByte(ByVal v As Integer) Implements DataOutput.writeByte
            write(v)
        End Sub

        Public Sub writeShort(ByVal v As Integer) Implements DataOutput.writeShort
            scratch(0) = CByte((v >> 8) And &HFF)
            scratch(1) = CByte(v And &HFF)
            write(scratch, 0, 2)
        End Sub

        Public Sub writeChar(ByVal v As Integer) Implements DataOutput.writeChar
            writeShort(v)
        End Sub

        Public Sub writeInt(ByVal v As Integer) Implements DataOutput.writeInt
            scratch(0) = CByte((v >> 24) And &HFF)
            scratch(1) = CByte((v >> 16) And &HFF)
            scratch(2) = CByte((v >> 8) And &HFF)
            scratch(3) = CByte(v And &HFF)
            write(scratch, 0, 4)
        End Sub

        Public Sub writeLong(ByVal v As Long) Implements DataOutput.writeLong
            For i As Integer = 0 To 7
                scratch(i) = CByte((v >> (56 - 8 * i)) And &HFF)
            Next i
            write(scratch, 0, 8)
        End Sub

        Public Sub writeFloat(ByVal v As Single) Implements DataOutput.writeFloat
            writeInt(BitConverter.ToInt32(BitConverter.GetBytes(v), 0))
        End Sub

        Public Sub writeDouble(ByVal v As Double) Implements DataOutput.writeDouble
            writeLong(BitConverter.DoubleToInt64Bits(v))
        End Sub

        ''' <summary>Writes the low byte of every character (Java <c>writeBytes</c>).</summary>
        Public Sub writeBytes(ByVal s As String) Implements DataOutput.writeBytes
            For Each c As Char In s
                write(AscW(c) And &HFF)
            Next c
        End Sub

        ''' <summary>Writes every character as a big-endian 16-bit value (Java <c>writeChars</c>).</summary>
        Public Sub writeChars(ByVal s As String) Implements DataOutput.writeChars
            For Each c As Char In s
                writeChar(AscW(c))
            Next c
        End Sub

        ''' <summary>Writes the string with a 2-byte length prefix in modified UTF-8 (Java <c>writeUTF</c>).</summary>
        Public Sub writeUTF(ByVal s As String) Implements DataOutput.writeUTF
            Dim bytes() As Byte = EncodeModifiedUtf8(s)
            If bytes.Length > 65535 Then Throw New FormatException("encoded string too long: " & bytes.Length & " bytes")
            writeShort(bytes.Length)
            write(bytes, 0, bytes.Length)
        End Sub

        Friend Shared Function EncodeModifiedUtf8(ByVal s As String) As Byte()
            Using ms As New MemoryStream(s.Length * 2)
                For Each ch As Char In s
                    Dim c As Integer = AscW(ch)
                    If c >= 1 AndAlso c <= &H7F Then
                        ms.WriteByte(CByte(c))
                    ElseIf c <= &H7FF Then
                        ms.WriteByte(CByte(&HC0 Or ((c >> 6) And &H1F)))
                        ms.WriteByte(CByte(&H80 Or (c And &H3F)))
                    Else
                        ms.WriteByte(CByte(&HE0 Or ((c >> 12) And &HF)))
                        ms.WriteByte(CByte(&H80 Or ((c >> 6) And &H3F)))
                        ms.WriteByte(CByte(&H80 Or (c And &H3F)))
                    End If
                Next ch
                Return ms.ToArray()
            End Using
        End Function

        Public Sub flush()
            stream.Flush()
        End Sub

        Public Sub Close()
            Dispose()
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            stream.Flush()
            If Not leaveOpen Then stream.Dispose()
        End Sub
    End Class

End Namespace
