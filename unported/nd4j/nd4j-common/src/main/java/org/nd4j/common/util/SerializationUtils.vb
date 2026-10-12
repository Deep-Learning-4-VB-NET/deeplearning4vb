Imports System
Imports System.IO
Imports FileUtils = org.apache.commons.io.FileUtils

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

Namespace org.nd4j.common.util


	Public Class SerializationUtils

		Protected Friend Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> T readObject(File file)
		Public Shared Function readObject(Of T)(ByVal file As File) As T
			Try
				Dim ois As New ObjectInputStream(FileUtils.openInputStream(file))
				Dim ret As T = CType(ois.readObject(), T)
				ois.close()
				Return ret
			Catch e As Exception
				Throw New Exception(e)
			End Try

		End Function

		''' <summary>
		''' Reads an object from the given input stream </summary>
		''' <param name="is"> the input stream to read from </param>
		''' <returns> the read object </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> T readObject(InputStream is)
		Public Shared Function readObject(Of T)(ByVal [is] As Stream) As T
			Try
				Dim ois As New ObjectInputStream([is])
				Dim ret As T = CType(ois.readObject(), T)
				ois.close()
				Return ret
			Catch e As Exception
				Throw New Exception(e)
			End Try

		End Function



		''' <summary>
		''' Converts the given object to a byte array </summary>
		''' <param name="toSave"> the object to save </param>
		Public Shared Function toByteArray(ByVal toSave As Serializable) As SByte()
			Try
				Dim bos As New MemoryStream()
				Dim os As New ObjectOutputStream(bos)
				os.writeObject(toSave)
				Dim ret() As SByte = bos.toByteArray()
				os.close()
				Return ret
			Catch e As Exception
				Throw New Exception(e)
			End Try

		End Function

		''' <summary>
		''' Deserializes object from byte array </summary>
		''' <param name="bytes"> </param>
		''' @param <T>
		''' @return </param>
		Public Shared Function fromByteArray(Of T)(ByVal bytes() As SByte) As T
			Return readObject(New MemoryStream(bytes))
		End Function

		''' <summary>
		''' Deserializes object from byte array </summary>
		''' <param name="bytes"> </param>
		''' @param <T>
		''' @return </param>
		Public Shared Function deserialize(Of T)(ByVal bytes() As SByte) As T
			Return fromByteArray(bytes)
		End Function

		''' <summary>
		''' Deserializes object from InputStream </summary>
		''' <param name="bytes"> </param>
		''' @param <T>
		''' @return </param>
		Public Shared Function deserialize(Of T)(ByVal [is] As Stream) As T
			Return readObject([is])
		End Function

		''' <summary>
		''' Writes the object to the output stream
		''' THIS DOES NOT FLUSH THE STREAM </summary>
		''' <param name="toSave"> the object to save </param>
		''' <param name="writeTo"> the output stream to write to </param>
		Public Shared Sub writeObject(ByVal toSave As Serializable, ByVal writeTo As Stream)
			Try
				Dim os As New ObjectOutputStream(writeTo)
				os.writeObject(toSave)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		''' Writes the object to the output stream
		''' THIS DOES NOT FLUSH THE STREAM </summary>
		''' <param name="toSave"> the object to save </param>
		''' <param name="writeTo"> the output stream to write to </param>
		Public Shared Sub serialize(ByVal [object] As Serializable, ByVal os As Stream)
			writeObject([object], os)
		End Sub

		Public Shared Sub saveObject(ByVal toSave As Object, ByVal saveTo As File)
			Try
				Dim os1 As Stream = FileUtils.openOutputStream(saveTo)
				Dim os As New ObjectOutputStream(os1)
				os.writeObject(toSave)
				os.flush()
				os.close()
				os1.Close()
			Catch e As Exception
				Throw New Exception(e)
			End Try

		End Sub
	End Class

End Namespace