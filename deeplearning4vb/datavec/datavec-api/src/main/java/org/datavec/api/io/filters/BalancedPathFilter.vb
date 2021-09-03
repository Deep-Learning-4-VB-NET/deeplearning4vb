Imports System
Imports System.Collections.Generic
Imports ParentPathLabelGenerator = org.datavec.api.io.labels.ParentPathLabelGenerator
Imports PathLabelGenerator = org.datavec.api.io.labels.PathLabelGenerator
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.io.filters


	Public Class BalancedPathFilter
		Inherits RandomPathFilter

		Protected Friend labelGenerator As PathLabelGenerator
		Protected Friend maxLabels As Long = 0, minPathsPerLabel As Long = 0, maxPathsPerLabel As Long = 0
		Protected Friend labels() As String = Nothing

		''' <summary>
		''' Calls {@code this(random, extensions, labelGenerator, 0, 0, 0, 0)}. </summary>
		Public Sub New(ByVal random As Random, ByVal extensions() As String, ByVal labelGenerator As PathLabelGenerator)
			Me.New(random, extensions, labelGenerator, 0, 0, 0, 0)
		End Sub

		''' <summary>
		''' Calls {@code this(random, null, labelGenerator, 0, 0, 0, maxPathsPerLabel)}. </summary>
		Public Sub New(ByVal random As Random, ByVal labelGenerator As PathLabelGenerator, ByVal maxPathsPerLabel As Long)
			Me.New(random, Nothing, labelGenerator, 0, 0, 0, maxPathsPerLabel)
		End Sub

		''' <summary>
		''' Calls {@code this(random, extensions, labelGenerator, 0, 0, 0, maxPathsPerLabel)}. </summary>
		Public Sub New(ByVal random As Random, ByVal extensions() As String, ByVal labelGenerator As PathLabelGenerator, ByVal maxPathsPerLabel As Long)
			Me.New(random, extensions, labelGenerator, 0, 0, 0, maxPathsPerLabel)
		End Sub

		''' <summary>
		''' Calls {@code this(random, extensions, labelGenerator, 0, maxLabels, 0, maxPathsPerLabel)}. </summary>
		Public Sub New(ByVal random As Random, ByVal labelGenerator As PathLabelGenerator, ByVal maxPaths As Long, ByVal maxLabels As Long, ByVal maxPathsPerLabel As Long)
			Me.New(random, Nothing, labelGenerator, maxPaths, maxLabels, 0, maxPathsPerLabel)
		End Sub

		''' <summary>
		''' Calls {@code this(random, extensions, labelGenerator, 0, maxLabels, 0, maxPathsPerLabel)}. </summary>
		Public Sub New(ByVal random As Random, ByVal extensions() As String, ByVal labelGenerator As PathLabelGenerator, ByVal maxLabels As Long, ByVal maxPathsPerLabel As Long)
			Me.New(random, extensions, labelGenerator, 0, maxLabels, 0, maxPathsPerLabel)
		End Sub

		''' <summary>
		''' Constructs an instance of the PathFilter. If {@code minPathsPerLabel > 0},
		''' it might return an unbalanced set if the value is larger than the number of
		''' examples available for the label with the minimum amount.
		''' </summary>
		''' <param name="random">           object to use </param>
		''' <param name="extensions">       of files to keep </param>
		''' <param name="labelGenerator">   to obtain labels from paths </param>
		''' <param name="maxPaths">         max number of paths to return (0 == unlimited) </param>
		''' <param name="maxLabels">        max number of labels to return (0 == unlimited) </param>
		''' <param name="minPathsPerLabel"> min number of paths per labels to return </param>
		''' <param name="maxPathsPerLabel"> max number of paths per labels to return (0 == unlimited) </param>
		''' <param name="labels">           of the paths to keep (empty set == keep all paths) </param>
		Public Sub New(ByVal random As Random, ByVal extensions() As String, ByVal labelGenerator As PathLabelGenerator, ByVal maxPaths As Long, ByVal maxLabels As Long, ByVal minPathsPerLabel As Long, ByVal maxPathsPerLabel As Long, ParamArray ByVal labels() As String)
			MyBase.New(random, extensions, maxPaths)
			Me.labelGenerator = labelGenerator
			Me.maxLabels = maxLabels
			Me.minPathsPerLabel = minPathsPerLabel
			Me.maxPathsPerLabel = maxPathsPerLabel
			Me.labels = labels
		End Sub

		Protected Friend Overridable Function acceptLabel(ByVal name As String) As Boolean
			If labels Is Nothing OrElse labels.Length = 0 Then
				Return True
			End If
			For Each label As String In labels
				If name.Equals(label) Then
					Return True
				End If
			Next label
			Return False
		End Function

		Public Overrides Function filter(ByVal paths() As URI) As URI()
			paths = MyBase.filter(paths)
			If labelGenerator Is Nothing Then
				labelGenerator = New ParentPathLabelGenerator()
			End If
			Dim labelPaths As IDictionary(Of Writable, IList(Of URI)) = New LinkedHashMap(Of Writable, IList(Of URI))()
			For i As Integer = 0 To paths.Length - 1
				Dim path As URI = paths(i)
				Dim label As Writable = labelGenerator.getLabelForPath(path)
				If Not acceptLabel(label.ToString()) Then
					Continue For
				End If
				Dim pathList As IList(Of URI) = labelPaths(label)
				If pathList Is Nothing Then
					If maxLabels > 0 AndAlso labelPaths.Count >= maxLabels Then
						Continue For
					End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: labelPaths.put(label, pathList = new ArrayList<java.net.URI>());
					pathList = New List(Of URI)()
						labelPaths(label) = pathList
				End If
				pathList.Add(path)
			Next i

			Dim minCount As Integer = If(maxPathsPerLabel > 0, CInt(Math.Min(maxPathsPerLabel, Integer.MaxValue)), Integer.MaxValue)
			For Each pathList As IList(Of URI) In labelPaths.Values
				If minCount > pathList.Count Then
					minCount = pathList.Count
				End If
			Next pathList
			If minCount < minPathsPerLabel Then
				minCount = CInt(Math.Min(minPathsPerLabel, Integer.MaxValue))
			End If

			Dim newpaths As New List(Of URI)()
			For i As Integer = 0 To minCount - 1
				For Each p As IList(Of URI) In labelPaths.Values
					If i < p.Count Then
						newpaths.Add(p(i))
					End If
				Next p
			Next i
			Return newpaths.ToArray()
		End Function
	End Class

End Namespace