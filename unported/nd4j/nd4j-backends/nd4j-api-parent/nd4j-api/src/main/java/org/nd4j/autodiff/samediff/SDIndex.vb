Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports ND4JIllegalArgumentException = org.nd4j.linalg.exception.ND4JIllegalArgumentException

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

Namespace org.nd4j.autodiff.samediff

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public class SDIndex
	Public Class SDIndex

		Public Enum IndexType
		  ALL
		  POINT
		  INTERVAL
		End Enum

		Private indexType As IndexType = IndexType.ALL

		Private Class ArgumentInterceptorAnonymousInnerClass
			Implements ArgumentInterceptor

			Private ReadOnly outerInstance As SameDiff

			Private pred As org.nd4j.autodiff.samediff.SDVariable
			Private switches As IDictionary(Of String, SDVariable())
			Private declared As ISet(Of String)

			Public Sub New(ByVal outerInstance As SameDiff, ByVal pred As org.nd4j.autodiff.samediff.SDVariable, ByVal switches As IDictionary(Of String, SDVariable()), ByVal declared As ISet(Of String))
				Me.outerInstance = outerInstance
				Me.pred = pred
				Me.switches = switches
				Me.declared = declared
			End Sub

			Public Function intercept(ByVal argument As SDVariable) As SDVariable Implements ArgumentInterceptor.intercept

				' if its declared in the if, we don't care acout it
				If Not declared.contains(argument.name()) Then
					Return argument
				End If

				' if we've already added a switch, move on
				If switches.containsKey(argument.name()) Then
					Return switches.get(argument.name())(1)
				End If

				Dim s() As SDVariable = switchOp(argument, pred)
				switches.put(argument.name(), s)
				Return s(1)
			End Function
		End Class

		Private Class ArgumentInterceptorAnonymousInnerClass2
			Implements ArgumentInterceptor

			Private ReadOnly outerInstance As SameDiff

			Private pred As org.nd4j.autodiff.samediff.SDVariable
			Private switches As IDictionary(Of String, SDVariable())
			Private declared2 As ISet(Of String)

			Public Sub New(ByVal outerInstance As SameDiff, ByVal pred As org.nd4j.autodiff.samediff.SDVariable, ByVal switches As IDictionary(Of String, SDVariable()), ByVal declared2 As ISet(Of String))
				Me.outerInstance = outerInstance
				Me.pred = pred
				Me.switches = switches
				Me.declared2 = declared2
			End Sub

			Public Function intercept(ByVal argument As SDVariable) As SDVariable Implements ArgumentInterceptor.intercept

				' if its declared in the if, we don't care acout it
				If Not declared2.contains(argument.name()) Then
					Return argument
				End If

				' if we've already added a switch, move on
				If switches.containsKey(argument.name()) Then
					Return switches.get(argument.name())(0)
				End If

				Dim s() As SDVariable = switchOp(argument, pred)
				switches.put(argument.name(), s)
				Return s(0)
			End Function
		End Class

		Private Class ArgumentInterceptorAnonymousInnerClass3
			Implements ArgumentInterceptor

			Private ReadOnly outerInstance As SameDiff

			Private frameName As String
			Private alreadyEntered As ISet(Of String)
			Private declared As ISet(Of String)
			Private done As IDictionary(Of String, SDVariable)
			Private sd As org.nd4j.autodiff.samediff.SameDiff

			Public Sub New(ByVal outerInstance As SameDiff, ByVal frameName As String, ByVal alreadyEntered As ISet(Of String), ByVal declared As ISet(Of String), ByVal done As IDictionary(Of String, SDVariable), ByVal sd As org.nd4j.autodiff.samediff.SameDiff)
				Me.outerInstance = outerInstance
				Me.frameName = frameName
				Me.alreadyEntered = alreadyEntered
				Me.declared = declared
				Me.done = done
				Me.sd = sd
			End Sub

			Public Function intercept(ByVal argument As SDVariable) As SDVariable Implements ArgumentInterceptor.intercept

				If Not declared.contains(argument.name()) Then
					Return argument
				End If

				If alreadyEntered.contains(argument.name()) Then
					Return argument
				End If

				If done.containsKey(argument.name()) Then
					Return done.get(argument.name())
				End If

				Dim e As SDVariable = (New Enter(sd, frameName, argument, True)).outputVariable()
				done.put(argument.name(), e)
				Return e
			End Function
		End Class
		Private pointIndex As Long
		Private pointKeepDim As Boolean
		Private intervalBegin As Long? = Nothing
		Private intervalEnd As Long? = Nothing
		Private intervalStrides As Long? = 1l


		Public Sub New()
		End Sub

		Public Shared Function all() As SDIndex
			Return New SDIndex()
		End Function


		Public Shared Function point(ByVal i As Long) As SDIndex
			Dim sdIndex As New SDIndex()
			sdIndex.indexType = IndexType.POINT
			sdIndex.pointIndex = i
			sdIndex.pointKeepDim = False
			Return sdIndex
		End Function


		Public Shared Function point(ByVal i As Long, ByVal keepDim As Boolean) As SDIndex
			Dim sdIndex As New SDIndex()
			sdIndex.indexType = IndexType.POINT
			sdIndex.pointIndex = i
			sdIndex.pointKeepDim = keepDim
			Return sdIndex
		End Function

		Public Shared Function interval(ByVal begin As Long?, ByVal [end] As Long?) As SDIndex
			Dim sdIndex As New SDIndex()
			sdIndex.indexType = IndexType.INTERVAL
			sdIndex.intervalBegin = begin
			sdIndex.intervalEnd = [end]
			Return sdIndex
		End Function

		Public Shared Function interval(ByVal begin As Integer?, ByVal [end] As Integer?) As SDIndex
			Dim sdIndex As New SDIndex()
			sdIndex.indexType = IndexType.INTERVAL
			If begin IsNot Nothing Then
				sdIndex.intervalBegin = begin.Value
			End If
			If [end] IsNot Nothing Then
				sdIndex.intervalEnd = [end].Value
			End If
			Return sdIndex
		End Function

		Public Shared Function interval(ByVal begin As Long?, ByVal strides As Long?, ByVal [end] As Long?) As SDIndex
			If strides = 0 Then
				Throw New ND4JIllegalArgumentException("Invalid index : strides can not be 0.")
			End If
			Dim sdIndex As New SDIndex()
			sdIndex.indexType = IndexType.INTERVAL
			sdIndex.intervalBegin = begin
			sdIndex.intervalEnd = [end]
			sdIndex.intervalStrides = strides
			Return sdIndex
		End Function

		Public Shared Function interval(ByVal begin As Integer?, ByVal strides As Integer?, ByVal [end] As Integer?) As SDIndex
			If strides = 0 Then
				Throw New ND4JIllegalArgumentException("Invalid index : strides can not be 0.")
			End If
			Dim sdIndex As New SDIndex()
			sdIndex.indexType = IndexType.INTERVAL
			If begin IsNot Nothing Then
				sdIndex.intervalBegin = begin.Value
			End If
			If [end] IsNot Nothing Then
				sdIndex.intervalEnd = [end].Value
			End If
			If strides IsNot Nothing Then
				sdIndex.intervalStrides = strides.Value
			End If
			Return sdIndex
		End Function

	End Class

End Namespace