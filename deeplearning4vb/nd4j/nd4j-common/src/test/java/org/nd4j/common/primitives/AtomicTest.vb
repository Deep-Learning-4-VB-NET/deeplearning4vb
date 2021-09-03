Imports System.IO
Imports val = lombok.val
Imports Test = org.junit.jupiter.api.Test
Imports org.nd4j.common.primitives
Imports SerializationUtils = org.nd4j.common.util.SerializationUtils
Imports org.junit.jupiter.api.Assertions

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

Namespace org.nd4j.common.primitives


	Public Class AtomicTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEquality_1()
		Public Overridable Sub testEquality_1()
			Dim v0 As val = New Atomic(Of Integer)(1327541)
			Dim v1 As val = New Atomic(Of Integer)(1327541)
			Dim v3 As val = New Atomic(Of Integer)(1327542)

			assertEquals(v0, v1)
			assertNotEquals(v0, v3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSerialization_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSerialization_1()
			Dim v0 As val = New Atomic(Of Integer)(1327541)

			Using baos As lombok.val = New MemoryStream()
				SerializationUtils.serialize(v0, baos)

				Using bais As lombok.val = New MemoryStream(baos.toByteArray())
					Dim v1 As Atomic(Of Integer) = SerializationUtils.deserialize(bais)

					assertEquals(v1, v0)
				End Using
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCas_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCas_1()
			Dim v0 As val = New Atomic(Of String)()

			v0.cas(Nothing, "alpha")
			assertEquals("alpha", v0.get())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCas_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCas_2()
			Dim v0 As val = New Atomic(Of String)("beta")

			v0.cas(Nothing, "alpha")
			assertEquals("beta", v0.get())
		End Sub
	End Class
End Namespace