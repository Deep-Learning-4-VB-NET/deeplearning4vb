Imports System
Imports lombok

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

Namespace org.nd4j.common.primitives


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor @Builder public class ImmutableQuad<F, S, T, O> implements java.io.Serializable
	<Serializable>
	Public Class ImmutableQuad(Of F, S, T, O)
		Private Const serialVersionUID As Long = 119L

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) protected F first;
		Protected Friend first As F
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) protected S second;
		Protected Friend second As S
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) protected T third;
		Protected Friend third As T
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) protected O fourth;
		Protected Friend fourth As O

		Public Shared Function quadOf(Of F, S, T, O)(ByVal first As F, ByVal second As S, ByVal third As T, ByVal fourth As O) As ImmutableQuad(Of F, S, T, O)
			Return New ImmutableQuad(Of F, S, T, O)(first, second, third, fourth)
		End Function

		Public Shared Function [of](Of F, S, T, O)(ByVal first As F, ByVal second As S, ByVal third As T, ByVal fourth As O) As ImmutableQuad(Of F, S, T, O)
			Return New ImmutableQuad(Of F, S, T, O)(first, second, third, fourth)
		End Function
	End Class

End Namespace