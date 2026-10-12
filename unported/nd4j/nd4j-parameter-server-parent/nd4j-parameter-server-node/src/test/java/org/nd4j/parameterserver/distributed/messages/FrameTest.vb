Imports System
Imports UnsafeBuffer = org.agrona.concurrent.UnsafeBuffer
Imports org.junit.jupiter.api
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports NodeRole = org.nd4j.parameterserver.distributed.enums.NodeRole
Imports Clipboard = org.nd4j.parameterserver.distributed.logic.completion.Clipboard
Imports Storage = org.nd4j.parameterserver.distributed.logic.Storage
Imports SkipGramRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.SkipGramRequestMessage
Imports org.nd4j.parameterserver.distributed.training
Imports Transport = org.nd4j.parameterserver.distributed.transport.Transport
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

Namespace org.nd4j.parameterserver.distributed.messages


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Deprecated @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class FrameTest extends org.nd4j.common.tests.BaseND4JTest
	<Obsolete>
	Public Class FrameTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()

		End Sub

		''' <summary>
		''' Simple test for Frame functionality
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000L) public void testFrame1()
		Public Overridable Sub testFrame1()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger count = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim count As New AtomicInteger(0)

			Dim frame As New Frame(Of TrainingMessage)()

			For i As Integer = 0 To 9
				frame.stackMessage(New TrainingMessageAnonymousInnerClass(Me, count))
			Next i

			assertEquals(10, frame.size())

			frame.processMessage()

			assertEquals(20, count.get())
		End Sub

		Private Class TrainingMessageAnonymousInnerClass
			Implements TrainingMessage

			Private ReadOnly outerInstance As FrameTest

			Private count As AtomicInteger

			Public Sub New(ByVal outerInstance As FrameTest, ByVal count As AtomicInteger)
				Me.outerInstance = outerInstance
				Me.count = count
			End Sub

			Public ReadOnly Property Counter As SByte Implements TrainingMessage.getCounter
				Get
					Return 2
				End Get
			End Property

			Public Property TargetId Implements org.nd4j.parameterserver.distributed.messages.VoidMessage.setTargetId As Short
				Set(ByVal id As Short)
    
				End Set
				Get
					Return 0
				End Get
			End Property

			Public ReadOnly Property RetransmitCount As Integer Implements org.nd4j.parameterserver.distributed.messages.VoidMessage.getRetransmitCount
				Get
					Return 0
				End Get
			End Property

			Public Sub incrementRetransmitCount() Implements org.nd4j.parameterserver.distributed.messages.VoidMessage.incrementRetransmitCount

			End Sub

			Public Property FrameId As Long Implements TrainingMessage.getFrameId
				Get
					Return 0
				End Get
				Set(ByVal frameId As Long)
    
				End Set
			End Property


			Public Property OriginatorId As Long Implements org.nd4j.parameterserver.distributed.messages.VoidMessage.getOriginatorId
				Get
					Return 0
				End Get
				Set(ByVal id As Long)
    
				End Set
			End Property



			Public ReadOnly Property TaskId As Long Implements org.nd4j.parameterserver.distributed.messages.VoidMessage.getTaskId
				Get
					Return 0
				End Get
			End Property

			Public ReadOnly Property MessageType As Integer Implements org.nd4j.parameterserver.distributed.messages.VoidMessage.getMessageType
				Get
					Return 0
				End Get
			End Property

			Public Function asBytes() As SByte() Implements org.nd4j.parameterserver.distributed.messages.VoidMessage.asBytes
				Return New SByte(){}
			End Function

			Public Function asUnsafeBuffer() As UnsafeBuffer Implements org.nd4j.parameterserver.distributed.messages.VoidMessage.asUnsafeBuffer
				Return Nothing
			End Function

			Public Sub attachContext(Of T1 As TrainingMessage)(ByVal voidConfiguration As VoidConfiguration, ByVal trainer As TrainingDriver(Of T1), ByVal clipboard As Clipboard, ByVal transport As Transport, ByVal storage As Storage, ByVal role As NodeRole, ByVal shardIndex As Short) Implements org.nd4j.parameterserver.distributed.messages.VoidMessage.attachContext
				' no-op intentionally
			End Sub

			Public Sub extractContext(ByVal message As BaseVoidMessage) Implements org.nd4j.parameterserver.distributed.messages.VoidMessage.extractContext
				' no-op intentionally
			End Sub

			Public Sub processMessage() Implements org.nd4j.parameterserver.distributed.messages.VoidMessage.processMessage
				count.incrementAndGet()
			End Sub

			Public ReadOnly Property JoinSupported As Boolean Implements org.nd4j.parameterserver.distributed.messages.VoidMessage.isJoinSupported
				Get
					Return False
				End Get
			End Property

			Public Sub joinMessage(ByVal message As VoidMessage) Implements org.nd4j.parameterserver.distributed.messages.VoidMessage.joinMessage
				' no-op
			End Sub

			Public ReadOnly Property BlockingMessage As Boolean Implements org.nd4j.parameterserver.distributed.messages.VoidMessage.isBlockingMessage
				Get
					Return False
				End Get
			End Property
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000L) public void testJoin1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testJoin1()
			Dim sgrm As New SkipGramRequestMessage(0, 1, New Integer() {3, 4, 5}, New SByte() {0, 1, 0}, CShort(0), 0.01, 119L)
			Dim frame As New Frame(Of SkipGramRequestMessage)(sgrm)
			For i As Integer = 0 To 9
				frame.stackMessage(sgrm)
			Next i

			' all messages should be stacked into one message
			assertEquals(1, frame.size())
			assertEquals(11, sgrm.Counter)
		End Sub
	End Class

End Namespace