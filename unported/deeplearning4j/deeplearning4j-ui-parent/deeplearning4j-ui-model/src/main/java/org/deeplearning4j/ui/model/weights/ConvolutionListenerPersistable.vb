Imports System
Imports System.IO
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports IOUtils = org.apache.commons.compress.utils.IOUtils
Imports Persistable = org.deeplearning4j.core.storage.Persistable

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.ui.model.weights


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public class ConvolutionListenerPersistable implements org.deeplearning4j.core.storage.Persistable
	<Serializable>
	Public Class ConvolutionListenerPersistable
		Implements Persistable

		Private Const TYPE_ID As String = "ConvolutionalListener"

'JAVA TO VB CONVERTER NOTE: The field sessionID was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private sessionID_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field workerID was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private workerID_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field timestamp was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private timestamp_Conflict As Long
		<NonSerialized>
		Private img As BufferedImage

		Public Sub New()
		End Sub

		Public Overridable ReadOnly Property SessionID As String Implements Persistable.getSessionID
			Get
				Return sessionID_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property TypeID As String Implements Persistable.getTypeID
			Get
				Return TYPE_ID
			End Get
		End Property

		Public Overridable ReadOnly Property WorkerID As String Implements Persistable.getWorkerID
			Get
				Return workerID_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property TimeStamp As Long Implements Persistable.getTimeStamp
			Get
				Return timestamp_Conflict
			End Get
		End Property

		Public Overridable Function encodingLengthBytes() As Integer Implements Persistable.encodingLengthBytes
			Return 0
		End Function

		Public Overridable Function encode() As SByte() Implements Persistable.encode
			'Not the most efficient: but it's easy to implement...
			Dim baos As New MemoryStream()
			Try
					Using oos As New ObjectOutputStream(baos)
					oos.writeObject(Me)
					End Using
			Catch e As IOException
				Throw New Exception(e) 'Shouldn't normally happen
			End Try

			Return baos.toByteArray()
		End Function

		Public Overridable Sub encode(ByVal buffer As ByteBuffer) Implements Persistable.encode
			buffer.put(encode())
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void encode(OutputStream outputStream) throws IOException
		Public Overridable Sub encode(ByVal outputStream As Stream)
			outputStream.Write(encode(), 0, encode().Length)
		End Sub

		Public Overridable Sub decode(ByVal decode() As SByte) Implements Persistable.decode
			Try
					Using ois As New ObjectInputStream(New MemoryStream(decode))
					Dim p As ConvolutionListenerPersistable = CType(ois.readObject(), ConvolutionListenerPersistable)
					Me.sessionID_Conflict = p.sessionID_Conflict
					Me.workerID_Conflict = p.workerID_Conflict
					Me.timestamp_Conflict = p.TimeStamp
					Me.img = p.img
					End Using
			Catch e As Exception When TypeOf e Is IOException OrElse TypeOf e Is ClassNotFoundException
				Throw New Exception(e) 'Shouldn't normally happen
			End Try
		End Sub

		Public Overridable Sub decode(ByVal buffer As ByteBuffer) Implements Persistable.decode
			Dim arr(buffer.remaining() - 1) As SByte
			buffer.get(arr)
			decode(arr)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void decode(InputStream inputStream) throws IOException
		Public Overridable Sub decode(ByVal inputStream As Stream)
			Dim b() As SByte = IOUtils.toByteArray(inputStream)
			decode(b)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void writeObject(ObjectOutputStream oos) throws IOException
		Private Sub writeObject(ByVal oos As ObjectOutputStream)
			oos.defaultWriteObject()
			ImageIO.write(img, "png", oos)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void readObject(ObjectInputStream ois) throws IOException, ClassNotFoundException
		Private Sub readObject(ByVal ois As ObjectInputStream)
			ois.defaultReadObject()
			img = ImageIO.read(ois)
		End Sub
	End Class

End Namespace