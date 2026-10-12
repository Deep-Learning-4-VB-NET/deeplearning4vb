Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter

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

Namespace org.deeplearning4j.text.documentiterator


	<Serializable>
	Public Class LabelsSource
		Private counter As New AtomicLong(0)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter private String template;
		Private template As String
		Private useFormatter As Boolean = False
		Private labels As IList(Of String)
		Private maxCount As Long = 0
		Private uniq As ISet(Of String) = Collections.newSetFromMap(New ConcurrentDictionary(Of String, Boolean)())

		Public Sub New()

		End Sub

		''' <summary>
		''' Build LabelsSource using string template.
		''' Template can be raw string, in this case document counter will be appended to resulting label.
		''' I.e. "SENT_" template will produce labels SENT_0, SENT_1, SENT_2 etc.
		''' 
		''' You can also use %d formatter tag, which will be replaced with counter.
		''' I.e. "SENT_%i_FLOW_1" will produce labels "SENT_0_FLOW_1", "SENT_1_FLOW_1", "SENT_2_FLOW_1" etc
		''' </summary>
		''' <param name="template"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LabelsSource(@NonNull String template)
		Public Sub New(ByVal template As String)
			Me.template = template
			If Me.template.Contains("%d") Then
				useFormatter = True
			End If
		End Sub

		Public Overridable Function indexOf(ByVal label As String) As Integer
			Return labels.IndexOf(label)
		End Function

		Public Overridable Function size() As Integer
			Return labels.Count
		End Function

		''' <summary>
		''' Build LabelsSource using externally defined list of string labels.
		''' Please note, in this case you have to make sure, the number of documents and number of labels match.
		''' </summary>
		''' <param name="labels"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LabelsSource(@NonNull List<String> labels)
		Public Sub New(ByVal labels As IList(Of String))
			Me.labels = New List(Of String)(labels)
			uniq.addAll(labels)
		End Sub

		''' <summary>
		''' Returns next label.
		''' </summary>
		''' <returns> next label, generated or predefined one </returns>
		Public Overridable Function nextLabel() As String
			SyncLock Me
				If labels IsNot Nothing Then
					Return labels(CType(counter.getAndIncrement(), Long?).Value)
				Else
					maxCount = counter.getAndIncrement()
					Return formatLabel(maxCount)
				End If
			End SyncLock
		End Function

		Private Function formatLabel(ByVal value As Long) As String
			If useFormatter Then
				Return String.format(template, value)
			Else
				Return template + value
			End If
		End Function

		''' <summary>
		''' This method returns the list of labels used by this generator instance.
		''' If external list os labels was used as source, whole list will be returned.
		''' </summary>
		''' <returns> list of labels </returns>
		Public Overridable ReadOnly Property Labels As IList(Of String)
			Get
				If labels IsNot Nothing AndAlso labels.Count > 0 Then
					Return labels
				Else
					Dim result As IList(Of String) = New List(Of String)()
					Dim x As Long = 0
					Do While x < counter.get()
						result.Add(formatLabel(x))
						x += 1
					Loop
					Return result
				End If
			End Get
		End Property

		''' <summary>
		''' This method is intended for storing labels retrieved from external sources.
		''' </summary>
		''' <param name="label"> </param>
		Public Overridable Sub storeLabel(ByVal label As String)
			If labels Is Nothing Then
				labels = New List(Of String)()
			End If

			If Not uniq.Contains(label) Then
				uniq.Add(label)
				labels.Add(label)
			End If
		End Sub

		''' <summary>
		''' This method should be called from Iterator's reset() method, to keep labels in sync with iterator
		''' </summary>
		Public Overridable Sub reset()
			Me.counter.set(0)
		End Sub

		''' <summary>
		''' This method returns number of labels used up to the method's call
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property NumberOfLabelsUsed As Integer
			Get
				If labels IsNot Nothing AndAlso labels.Count > 0 Then
					Return labels.Count
				Else
					Return CType(maxCount + 1, Long?).Value
				End If
			End Get
		End Property
	End Class

End Namespace