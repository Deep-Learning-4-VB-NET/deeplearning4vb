﻿Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.nd4j.autodiff.samediff.internal


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class IdentityDependencyTracker<T, D> extends AbstractDependencyTracker<T,D>
	Public Class IdentityDependencyTracker(Of T, D)
		Inherits AbstractDependencyTracker(Of T, D)

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: @Override protected Map<T, ?> newTMap()
		Protected Friend Overrides Function newTMap() As IDictionary(Of T, Object)
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: return new IdentityHashMap<>();
			Return New IdentityHashMap(Of T, Object)()
		End Function

		Protected Friend Overrides Function newTSet() As ISet(Of T)
			Return Collections.newSetFromMap(New IdentityHashMap(Of T, Boolean)())
		End Function

		Protected Friend Overrides Function toStringT(ByVal t As T) As String
			If TypeOf t Is INDArray Then
				Dim i As INDArray = DirectCast(t, INDArray)
				Return System.identityHashCode(t) & " - id=" & i.Id & ", " & i.shapeInfoToString()
			Else
				Return System.identityHashCode(t) & " - " & t.ToString()
			End If
		End Function

		Protected Friend Overrides Function toStringD(ByVal d As D) As String
			Return d.ToString()
		End Function
	End Class

End Namespace