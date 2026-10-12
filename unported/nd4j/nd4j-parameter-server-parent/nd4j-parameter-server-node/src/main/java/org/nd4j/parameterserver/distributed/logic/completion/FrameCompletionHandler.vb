Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Slf4j = lombok.extern.slf4j.Slf4j

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

Namespace org.nd4j.parameterserver.distributed.logic.completion


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Deprecated public class FrameCompletionHandler
	<Obsolete>
	Public Class FrameCompletionHandler

		Private frames As IDictionary(Of RequestDescriptor, FrameDescriptor) = New ConcurrentDictionary(Of RequestDescriptor, FrameDescriptor)()

		Public Overridable Function isTrackingFrame(ByVal descriptor As RequestDescriptor) As Boolean
			Return frames.ContainsKey(descriptor)
		End Function

		Public Overridable Function isTrackingFrame(ByVal originatorId As Long, ByVal frameId As Long) As Boolean
			Return frames.ContainsKey(RequestDescriptor.createDescriptor(originatorId, frameId))
		End Function

		''' 
		''' 
		''' <param name="originatorId"> </param>
		''' <param name="frameId"> </param>
		''' <param name="messageId"> </param>
		Public Overridable Sub addHook(ByVal originatorId As Long?, ByVal frameId As Long?, ByVal messageId As Long?)
			Dim descriptor As RequestDescriptor = RequestDescriptor.createDescriptor(originatorId, frameId)
			If Not frames.ContainsKey(descriptor) Then
				frames(descriptor) = New FrameDescriptor(originatorId)
			End If

			frames(descriptor).addMessage(messageId)
		End Sub

		Public Overridable Sub notifyFrame(ByVal descriptor As RequestDescriptor, ByVal messageId As Long?)
			Dim frameDescriptor As FrameDescriptor = frames(descriptor)

			If frameDescriptor IsNot Nothing Then
				frameDescriptor.finishedMessage(messageId)
			End If
		End Sub

		Public Overridable Sub notifyFrame(ByVal originatorId As Long?, ByVal frameId As Long?, ByVal messageId As Long?)
			notifyFrame(RequestDescriptor.createDescriptor(originatorId, frameId), messageId)
		End Sub

		Public Overridable Function isCompleted(ByVal descriptor As RequestDescriptor) As Boolean
			If isTrackingFrame(descriptor) Then
				' FIXME: double spending possible here
				Dim frameDescriptor As FrameDescriptor = frames(descriptor)
				If frameDescriptor Is Nothing Then
					Return False
				End If

				Return frameDescriptor.Finished
			Else
				log.warn("DOUBLE SPENDING!!!")
				Return False
			End If
		End Function

		Public Overridable Function isCompleted(ByVal originatorId As Long, ByVal frameId As Long) As Boolean
			Dim descriptor As RequestDescriptor = RequestDescriptor.createDescriptor(originatorId, frameId)
			Return isCompleted(descriptor)
		End Function

		Public Overridable Function getCompletedFrameInfo(ByVal descriptor As RequestDescriptor) As FrameDescriptor
			Try
				Return frames(descriptor)
			Finally
				frames.Remove(descriptor)
			End Try
		End Function

		Public Overridable Function getCompletedFrameInfo(ByVal originatorId As Long, ByVal frameId As Long) As FrameDescriptor
			Dim descriptor As RequestDescriptor = RequestDescriptor.createDescriptor(originatorId, frameId)
			Return getCompletedFrameInfo(descriptor)
		End Function


		Public Class FrameDescriptor

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long frameOriginatorId;
			Friend frameOriginatorId As Long

			' messageId within frame, and it's state
			Friend states As IDictionary(Of Long, AtomicBoolean) = New ConcurrentDictionary(Of Long, AtomicBoolean)()
			Friend messages As New AtomicInteger(0)
'JAVA TO VB CONVERTER NOTE: The field finished was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend finished_Conflict As New AtomicInteger(0)


			Public Sub New(ByVal frameOriginatorId As Long)
				Me.frameOriginatorId = frameOriginatorId
			End Sub

			Public Overridable ReadOnly Property Finished As Boolean
				Get
					Return messages.get() = finished_Conflict.get()
				End Get
			End Property

			Public Overridable Sub addMessage(ByVal messageId As Long?)
				states(messageId) = New AtomicBoolean(False)
				messages.incrementAndGet()
			End Sub

			Public Overridable Sub finishedMessage(ByVal messageId As Long?)
				Dim boo As AtomicBoolean = states(messageId)
				If boo IsNot Nothing Then
					boo.set(True)
				End If

				finished_Conflict.incrementAndGet()
			End Sub

			Public Overridable ReadOnly Property IncompleteNumber As Integer
				Get
					Return messages.get() - finished_Conflict.get()
				End Get
			End Property
		End Class
	End Class

End Namespace