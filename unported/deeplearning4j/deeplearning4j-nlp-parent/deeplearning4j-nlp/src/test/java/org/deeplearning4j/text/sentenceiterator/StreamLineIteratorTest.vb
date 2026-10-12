Imports System.IO
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Test = org.junit.jupiter.api.Test
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotEquals

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

Namespace org.deeplearning4j.text.sentenceiterator


	Public Class StreamLineIteratorTest
		Inherits BaseDL4JTest

		Protected Friend logger As Logger = LoggerFactory.getLogger(GetType(StreamLineIteratorTest))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHasNext() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testHasNext()

			Dim reuters5250 As New ClassPathResource("/reuters/5250")
			Dim f As File = reuters5250.File

			Dim iterator As StreamLineIterator = (New StreamLineIterator.Builder(New FileStream(f, FileMode.Open, FileAccess.Read))).setFetchSize(100).build()

			Dim cnt As Integer = 0
			Do While iterator.hasNext()
				Dim line As String = iterator.nextSentence()

				assertNotEquals(Nothing, line)
				logger.info("Line: " & line)
				cnt += 1
			Loop

			assertEquals(24, cnt)
		End Sub
	End Class

End Namespace