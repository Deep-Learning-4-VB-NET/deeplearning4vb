Imports System
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.nd4j.linalg



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class DataTypeTest extends BaseNd4jTestWithBackends
	Public Class DataTypeTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDataTypes(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDataTypes(ByVal backend As Nd4jBackend)
			For Each type As val In DataType.values()
				If DataType.UTF8.Equals(type) OrElse DataType.UNKNOWN.Equals(type) OrElse DataType.COMPRESSED.Equals(type) Then
					Continue For
				End If

				Dim in1 As val = Nd4j.ones(type, 10, 10)

				Dim baos As val = New MemoryStream()
				Dim oos As val = New ObjectOutputStream(baos)
				Try
					oos.writeObject(in1)
				Catch e As Exception
					Throw New Exception("Failed for data type [" & type & "]", e)
				End Try

				Dim bios As val = New MemoryStream(baos.toByteArray())
				Dim ois As val = New ObjectInputStream(bios)
				Try
					Dim in2 As val = DirectCast(ois.readObject(), INDArray)
					assertEquals(in1, in2,"Failed for data type [" & type & "]")
				Catch e As Exception
					Throw New Exception("Failed for data type [" & type & "]", e)
				End Try
			Next type
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace