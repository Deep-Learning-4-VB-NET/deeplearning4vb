Imports System
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports FrameCompletionHandler = org.nd4j.parameterserver.distributed.logic.completion.FrameCompletionHandler
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

Namespace org.nd4j.parameterserver.distributed.logic

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Deprecated @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class FrameCompletionHandlerTest extends org.nd4j.common.tests.BaseND4JTest
	<Obsolete>
	Public Class FrameCompletionHandlerTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()

		End Sub



		''' <summary>
		''' This test emulates 2 frames being processed at the same time </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCompletion1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCompletion1()
			Dim handler As New FrameCompletionHandler()
			Dim frames() As Long = {15L, 17L}
			Dim originators() As Long = {123L, 183L}
			For Each originator As Long? In originators
				For Each frame As Long? In frames
					For e As Integer = 1 To 512
						handler.addHook(originator, frame, CLng(e))
					Next e
				Next frame

				For Each frame As Long? In frames
					For e As Integer = 1 To 512
						handler.notifyFrame(originator, frame, CLng(e))
					Next e
				Next frame
			Next originator


			For Each originator As Long? In originators
				For Each frame As Long? In frames
					assertEquals(True, handler.isCompleted(originator, frame))
				Next frame
			Next originator
		End Sub

	End Class

End Namespace