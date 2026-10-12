Imports System.Collections.Generic

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

Namespace org.nd4j.common.util


	Public Class SetUtils
		Protected Friend Sub New()
		End Sub

		' Set specific operations

		Public Shared Function intersection(Of T)(ByVal parentCollection As ICollection(Of T), ByVal removeFromCollection As ICollection(Of T)) As ISet(Of T)
			Dim results As ISet(Of T) = New HashSet(Of T)(parentCollection)
			results.RetainAll(removeFromCollection)
			Return results
		End Function

		Public Shared Function intersectionP(Of T, T1 As T, T2 As T)(ByVal s1 As ISet(Of T1), ByVal s2 As ISet(Of T2)) As Boolean
			For Each elt As T In s1
				If s2.Contains(elt) Then
					Return True
				End If
			Next elt
			Return False
		End Function

		Public Shared Function union(Of T, T1 As T, T2 As T)(ByVal s1 As ISet(Of T1), ByVal s2 As ISet(Of T2)) As ISet(Of T)
			Dim s3 As ISet(Of T) = New HashSet(Of T)(s1)
			s3.addAll(s2)
			Return s3
		End Function

		''' <summary>
		''' Return is s1 \ s2 </summary>

		Public Shared Function difference(Of T, T1 As T, T2 As T)(ByVal s1 As ICollection(Of T1), ByVal s2 As ICollection(Of T2)) As ISet(Of T)
			Dim s3 As ISet(Of T) = New HashSet(Of T)(s1)
			s3.RemoveAll(s2)
			Return s3
		End Function
	End Class



End Namespace