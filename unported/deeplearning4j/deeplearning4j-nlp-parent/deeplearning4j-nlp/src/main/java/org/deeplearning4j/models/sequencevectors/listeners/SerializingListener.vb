Imports System
Imports System.Text
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.deeplearning4j.models.sequencevectors
Imports ListenerEvent = org.deeplearning4j.models.sequencevectors.enums.ListenerEvent
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports SerializationUtils = org.nd4j.common.util.SerializationUtils

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

Namespace org.deeplearning4j.models.sequencevectors.listeners


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SerializingListener<T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> implements org.deeplearning4j.models.sequencevectors.interfaces.VectorsListener<T>
	Public Class SerializingListener(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements VectorsListener(Of T)

		Private targetFolder As New File("./")
		Private modelPrefix As String = "Model_"
		Private useBinarySerialization As Boolean = True
		Private targetEvent As ListenerEvent = ListenerEvent.EPOCH
		Private targetFrequency As Integer = 100000

		Private locker As New Semaphore(1)

		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' This method is called prior each processEvent call, to check if this specific VectorsListener implementation is viable for specific event
		''' </summary>
		''' <param name="event"> </param>
		''' <param name="argument"> </param>
		''' <returns> TRUE, if this event can and should be processed with this listener, FALSE otherwise </returns>
		Public Overridable Function validateEvent(ByVal [event] As ListenerEvent, ByVal argument As Long) As Boolean Implements VectorsListener(Of T).validateEvent
			Try
				''' <summary>
				''' please note, since sequence vectors are multithreaded we need to stop processed while model is being saved
				''' </summary>
				locker.acquire()

				If [event] = targetEvent AndAlso argument Mod targetFrequency = 0 Then
					Return True
				Else
					Return False
				End If
			Catch e As Exception
				Throw New Exception(e)
			Finally
				locker.release()
			End Try
		End Function

		''' <summary>
		''' This method is called at each epoch end
		''' </summary>
		''' <param name="event"> </param>
		''' <param name="sequenceVectors"> </param>
		''' <param name="argument"> </param>
		Public Overridable Sub processEvent(ByVal [event] As ListenerEvent, ByVal sequenceVectors As SequenceVectors(Of T), ByVal argument As Long) Implements VectorsListener(Of T).processEvent
			Try
				locker.acquire()

				Dim sdf As New SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS")

				Dim builder As New StringBuilder(targetFolder.getAbsolutePath())
				builder.Append("/").Append(modelPrefix).Append("_").Append(sdf.format(DateTime.Now)).Append(".seqvec")
				Dim targetFile As New File(builder.ToString())

				If useBinarySerialization Then
					SerializationUtils.saveObject(sequenceVectors, targetFile)
				Else
					Throw New System.NotSupportedException("Not implemented yet")
				End If

			Catch e As Exception
				log.error("",e)
			Finally
				locker.release()
			End Try
		End Sub

		Public Class Builder(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
'JAVA TO VB CONVERTER NOTE: The field targetFolder was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend targetFolder_Conflict As New File("./")
			Friend modelPrefix As String = "Model_"
			Friend useBinarySerialization As Boolean = True
			Friend targetEvent As ListenerEvent = ListenerEvent.EPOCH
			Friend targetFrequency As Integer = 100000

			Public Sub New(ByVal targetEvent As ListenerEvent, ByVal frequency As Integer)
				Me.targetEvent = targetEvent
				Me.targetFrequency = frequency
			End Sub

			''' <summary>
			''' This method allows you to define template for file names that will be created during serialization </summary>
			''' <param name="reallyUse">
			''' @return </param>
			Public Overridable Function setFilenamePrefix(ByVal reallyUse As Boolean) As Builder(Of T)
				Me.useBinarySerialization = reallyUse
				Return Me
			End Function

			''' <summary>
			''' This method specifies target folder where models should be saved
			''' </summary>
			''' <param name="folder">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setTargetFolder(@NonNull String folder)
			Public Overridable Function setTargetFolder(ByVal folder As String) As Builder(Of T)
				Me.setTargetFolder(New File(folder))
				Return Me
			End Function

			''' <summary>
			''' This method specifies target folder where models should be saved
			''' </summary>
			''' <param name="folder">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setTargetFolder(@NonNull File folder)
			Public Overridable Function setTargetFolder(ByVal folder As File) As Builder(Of T)
				If Not folder.exists() OrElse Not folder.isDirectory() Then
					Throw New System.InvalidOperationException("Target folder must exist!")
				End If
				Me.targetFolder_Conflict = folder
				Return Me
			End Function

			''' <summary>
			''' This method returns new SerializingListener instance
			''' 
			''' @return
			''' </summary>
			Public Overridable Function build() As SerializingListener(Of T)
				Dim listener As New SerializingListener(Of T)()
				listener.modelPrefix = Me.modelPrefix
				listener.targetFolder = Me.targetFolder_Conflict
				listener.useBinarySerialization = Me.useBinarySerialization
				listener.targetEvent = Me.targetEvent
				listener.targetFrequency = Me.targetFrequency

				Return listener
			End Function
		End Class
	End Class

End Namespace