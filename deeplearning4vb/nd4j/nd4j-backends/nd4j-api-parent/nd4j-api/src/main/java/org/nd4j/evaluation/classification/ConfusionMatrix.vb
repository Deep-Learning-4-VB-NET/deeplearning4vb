Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports System.Linq
Imports HashMultiset = org.nd4j.shade.guava.collect.HashMultiset
Imports Multiset = org.nd4j.shade.guava.collect.Multiset
Imports Getter = lombok.Getter

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

Namespace org.nd4j.evaluation.classification


'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: public class ConfusionMatrix<T extends Comparable<? super T>> implements java.io.Serializable
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	<Serializable>
	Public Class ConfusionMatrix(Of T As IComparable(Of Object))
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private volatile java.util.Map<T, org.nd4j.shade.guava.collect.Multiset<T>> matrix;
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private matrix As IDictionary(Of T, Multiset(Of T))
		Private classes As IList(Of T)

		''' <summary>
		''' Creates an empty confusion Matrix
		''' </summary>
		Public Sub New(ByVal classes As IList(Of T))
			Me.matrix = New ConcurrentDictionary(Of T, Multiset(Of T))()
			Me.classes = classes
		End Sub

		Public Sub New()
			Me.New(New List(Of T)())
		End Sub

		''' <summary>
		''' Creates a new ConfusionMatrix initialized with the contents of another ConfusionMatrix.
		''' </summary>
		Public Sub New(ByVal other As ConfusionMatrix(Of T))
			Me.New(other.getClasses())
			Me.add(other)
		End Sub

		''' <summary>
		''' Increments the entry specified by actual and predicted by one.
		''' </summary>
		Public Overridable Sub add(ByVal actual As T, ByVal predicted As T)
			SyncLock Me
				add(actual, predicted, 1)
			End SyncLock
		End Sub

		''' <summary>
		''' Increments the entry specified by actual and predicted by count.
		''' </summary>
		Public Overridable Sub add(ByVal actual As T, ByVal predicted As T, ByVal count As Integer)
			SyncLock Me
				If matrix.ContainsKey(actual) Then
					matrix(actual).add(predicted, count)
				Else
					Dim counts As Multiset(Of T) = HashMultiset.create()
					counts.add(predicted, count)
					matrix(actual) = counts
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Adds the entries from another confusion matrix to this one.
		''' </summary>
		Public Overridable Sub add(ByVal other As ConfusionMatrix(Of T))
			SyncLock Me
				For Each actual As T In other.matrix.Keys
					Dim counts As Multiset(Of T) = other.matrix(actual)
					For Each predicted As T In counts.elementSet()
						Dim count As Integer = counts.count(predicted)
						Me.add(actual, predicted, count)
					Next predicted
				Next actual
			End SyncLock
		End Sub

		''' <summary>
		''' Gives the applyTransformToDestination of all classes in the confusion matrix.
		''' </summary>
		Public Overridable ReadOnly Property Classes As IList(Of T)
			Get
				If classes Is Nothing Then
					classes = New List(Of T)()
				End If
				Return classes
			End Get
		End Property

		''' <summary>
		''' Gives the count of the number of times the "predicted" class was predicted for the "actual"
		''' class.
		''' </summary>
		Public Overridable Function getCount(ByVal actual As T, ByVal predicted As T) As Integer
			SyncLock Me
				If Not matrix.ContainsKey(actual) Then
					Return 0
				Else
					Return matrix(actual).count(predicted)
				End If
			End SyncLock
		End Function

		''' <summary>
		''' Computes the total number of times the class was predicted by the classifier.
		''' </summary>
		Public Overridable Function getPredictedTotal(ByVal predicted As T) As Integer
			SyncLock Me
				Dim total As Integer = 0
				For Each actual As T In classes
					total += getCount(actual, predicted)
				Next actual
				Return total
			End SyncLock
		End Function

		''' <summary>
		''' Computes the total number of times the class actually appeared in the data.
		''' </summary>
		Public Overridable Function getActualTotal(ByVal actual As T) As Integer
			SyncLock Me
				If Not matrix.ContainsKey(actual) Then
					Return 0
				Else
					Dim total As Integer = 0
					For Each elem As T In matrix(actual).elementSet()
						total += matrix(actual).count(elem)
					Next elem
					Return total
				End If
			End SyncLock
		End Function

		Public Overrides Function ToString() As String
			Return matrix.ToString()
		End Function

		''' <summary>
		''' Outputs the ConfusionMatrix as comma-separated values for easy import into spreadsheets
		''' </summary>
		Public Overridable Function toCSV() As String
			Dim builder As New StringBuilder()

			' Header Row
			builder.Append(",,Predicted Class," & vbLf)

			' Predicted Classes Header Row
			builder.Append(",,")
			For Each predicted As T In classes
				builder.Append(String.Format("{0},", predicted))
			Next predicted
			builder.Append("Total" & vbLf)

			' Data Rows
			Dim firstColumnLabel As String = "Actual Class,"
			For Each actual As T In classes
				builder.Append(firstColumnLabel)
				firstColumnLabel = ","
				builder.Append(String.Format("{0},", actual))

				For Each predicted As T In classes
					builder.Append(getCount(actual, predicted))
					builder.Append(",")
				Next predicted
				' Actual Class Totals Column
				builder.Append(getActualTotal(actual))
				builder.Append(vbLf)
			Next actual

			' Predicted Class Totals Row
			builder.Append(",Total,")
			For Each predicted As T In classes
				builder.Append(getPredictedTotal(predicted))
				builder.Append(",")
			Next predicted
			builder.Append(vbLf)

			Return builder.ToString()
		End Function

		''' <summary>
		''' Outputs Confusion Matrix in an HTML table. Cascading Style Sheets (CSS) can control the table's
		''' appearance by defining the empty-space, actual-count-header, predicted-class-header, and
		''' count-element classes. For example
		''' </summary>
		''' <returns> html string </returns>
		Public Overridable Function toHTML() As String
			Dim builder As New StringBuilder()

			Dim numClasses As Integer = classes.Count
			' Header Row
			builder.Append("<table>" & vbLf)
			builder.Append("<tr><th class=""empty-space"" colspan=""2"" rowspan=""2"">")
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
			builder.Append(String.Format("<th class=""predicted-class-header"" colspan=""%d"">Predicted Class</th></tr>%n", numClasses + 1))

			' Predicted Classes Header Row
			builder.Append("<tr>")
			' builder.append("<th></th><th></th>");
			For Each predicted As T In classes
				builder.Append("<th class=""predicted-class-header"">")
				builder.Append(predicted)
				builder.Append("</th>")
			Next predicted
			builder.Append("<th class=""predicted-class-header"">Total</th>")
			builder.Append("</tr>" & vbLf)

			' Data Rows
			Dim firstColumnLabel As String = String.Format("<tr><th class=""actual-class-header"" rowspan=""{0:D}"">Actual Class</th>", numClasses + 1)
			For Each actual As T In classes
				builder.Append(firstColumnLabel)
				firstColumnLabel = "<tr>"
				builder.Append(String.Format("<th class=""actual-class-header"" >{0}</th>", actual))

				For Each predicted As T In classes
					builder.Append("<td class=""count-element"">")
					builder.Append(getCount(actual, predicted))
					builder.Append("</td>")
				Next predicted

				' Actual Class Totals Column
				builder.Append("<td class=""count-element"">")
				builder.Append(getActualTotal(actual))
				builder.Append("</td>")
				builder.Append("</tr>" & vbLf)
			Next actual

			' Predicted Class Totals Row
			builder.Append("<tr><th class=""actual-class-header"">Total</th>")
			For Each predicted As T In classes
				builder.Append("<td class=""count-element"">")
				builder.Append(getPredictedTotal(predicted))
				builder.Append("</td>")
			Next predicted
			builder.Append("<td class=""empty-space""></td>" & vbLf)
			builder.Append("</tr>" & vbLf)
			builder.Append("</table>" & vbLf)

			Return builder.ToString()
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is ConfusionMatrix) Then
				Return False
			End If
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: ConfusionMatrix<?> c = (ConfusionMatrix<?>) o;
			Dim c As ConfusionMatrix(Of Object) = DirectCast(o, ConfusionMatrix(Of Object))
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: return matrix.equals(c.matrix) && classes.equals(c.classes);
			Return matrix.Equals(c.matrix) AndAlso classes.SequenceEqual(c.classes)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = 17
			result = 31 * result + (If(matrix Is Nothing, 0, matrix.GetHashCode()))
			result = 31 * result + (If(classes Is Nothing, 0, classes.GetHashCode()))
			Return result
		End Function
	End Class

End Namespace