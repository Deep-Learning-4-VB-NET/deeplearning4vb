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

Namespace org.nd4j.graph

	Public NotInheritable Class UIHistogramType
	  Private Sub New()
	  End Sub
	  Public Const DISCRETE As SByte = 0
	  Public Const EQUAL_SPACING As SByte = 1
	  Public Const CUSTOM As SByte = 2

	  Public Shared ReadOnly names() As String = { "DISCRETE", "EQUAL_SPACING", "CUSTOM"}

	  Public Shared Function name(ByVal e As Integer) As String
		  Return names(e)
	  End Function
	End Class


End Namespace