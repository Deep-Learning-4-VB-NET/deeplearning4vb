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

	Public NotInheritable Class UIEventType
	  Private Sub New()
	  End Sub
	  Public Const ADD_NAME As SByte = 0
	  Public Const SCALAR As SByte = 1
	  Public Const ARRAY As SByte = 2
	  Public Const ARRAY_LIST As SByte = 3
	  Public Const HISTOGRAM As SByte = 4
	  Public Const IMAGE As SByte = 5
	  Public Const SUMMARY_STATISTICS As SByte = 6
	  Public Const OP_TIMING As SByte = 7
	  Public Const HARDWARE_STATE As SByte = 8
	  Public Const GC_EVENT As SByte = 9

	  Public Shared ReadOnly names() As String = { "ADD_NAME", "SCALAR", "ARRAY", "ARRAY_LIST", "HISTOGRAM", "IMAGE", "SUMMARY_STATISTICS", "OP_TIMING", "HARDWARE_STATE", "GC_EVENT"}

	  Public Shared Function name(ByVal e As Integer) As String
		  Return names(e)
	  End Function
	End Class


End Namespace