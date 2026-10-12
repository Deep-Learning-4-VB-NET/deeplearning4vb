Imports System.Threading
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.deeplearning4j.spark.time

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestTimeSource
	Public Class TestTimeSource
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTimeSourceNTP() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTimeSourceNTP()
			Dim timeSource As TimeSource = TimeSourceProvider.Instance
			assertTrue(TypeOf timeSource Is NTPTimeSource)

			For i As Integer = 0 To 9
				Dim systemTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
				Dim ntpTime As Long = timeSource.currentTimeMillis()
				Dim offset As Long = ntpTime - systemTime
	'            System.out.println("System: " + systemTime + "\tNTPTimeSource: " + ntpTime + "\tOffset: " + offset);
				Thread.Sleep(500)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTimeSourceSystem() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTimeSourceSystem()
			Dim timeSource As TimeSource = TimeSourceProvider.getInstance("org.deeplearning4j.spark.time.SystemClockTimeSource")
			assertTrue(TypeOf timeSource Is SystemClockTimeSource)

			For i As Integer = 0 To 9
				Dim systemTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
				Dim ntpTime As Long = timeSource.currentTimeMillis()
				Dim offset As Long = ntpTime - systemTime
	'            System.out.println("System: " + systemTime + "\tSystemClockTimeSource: " + ntpTime + "\tOffset: " + offset);
				assertEquals(systemTime, ntpTime, 2) 'Should be exact, but we might randomly tick over between one ms and the next
				Thread.Sleep(500)
			Next i
		End Sub

	End Class

End Namespace