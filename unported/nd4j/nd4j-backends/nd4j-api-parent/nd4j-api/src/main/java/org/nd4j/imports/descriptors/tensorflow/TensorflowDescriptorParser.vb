Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports TextFormat = org.nd4j.shade.protobuf.TextFormat
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports OpDef = org.tensorflow.framework.OpDef

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

Namespace org.nd4j.imports.descriptors.tensorflow


	Public Class TensorflowDescriptorParser

		Protected Friend Shared DESCRIPTORS As IDictionary(Of String, OpDef)

		''' <summary>
		''' Get the op descriptors for tensorflow </summary>
		''' <returns> the op descriptors for tensorflow </returns>
		''' <exception cref="Exception"> </exception>
		Public Shared Function opDescs() As IDictionary(Of String, OpDef)
			SyncLock GetType(TensorflowDescriptorParser)
				If DESCRIPTORS IsNot Nothing Then
					Return DESCRIPTORS
				End If
        
				Try
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: Using contents As System.IO.Stream_Input = (new org.nd4j.common.io.ClassPathResource("ops.proto")).InputStream, bis2 As java.io.BufferedInputStream = new java.io.BufferedInputStream(contents), reader As System.IO.StreamReader_BufferedReader = new System.IO.StreamReader(bis2)
						New BufferedInputStream(contents), reader As New StreamReader(bis2)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: Using contents As System.IO.Stream_Input = (new org.nd4j.common.io.ClassPathResource("ops.proto")).InputStream, bis2 As java.io.BufferedInputStream = new java.io.BufferedInputStream(contents), reader As System.IO.StreamReader_BufferedReader
							(New ClassPathResource("ops.proto")).InputStream, bis2 As New BufferedInputStream(contents), reader As StreamReader
								Using contents As Stream = (New ClassPathResource("ops.proto")).InputStream, bis2 As BufferedInputStream
						Dim builder As org.tensorflow.framework.OpList.Builder = org.tensorflow.framework.OpList.newBuilder()
            
						Dim str As New StringBuilder()
						Dim line As String = Nothing
						line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
						Do While line IsNot Nothing
							str.Append(line) '.append("\n");
								line = reader.ReadLine()
						Loop
            
            
						TextFormat.getParser().merge(str.ToString(), builder)
						Dim list As IList(Of OpDef) = builder.getOpList()
						Dim map As IDictionary(Of String, OpDef) = New Dictionary(Of String, OpDef)()
						For Each opDef As OpDef In list
							map(opDef.getName()) = opDef
						Next opDef
            
						DESCRIPTORS = map
						Return DESCRIPTORS
						End Using
				Catch e As Exception
					Throw New ND4JIllegalStateException("Unable to load tensorflow descriptors", e)
				End Try
        
			End SyncLock
		End Function
	End Class

End Namespace