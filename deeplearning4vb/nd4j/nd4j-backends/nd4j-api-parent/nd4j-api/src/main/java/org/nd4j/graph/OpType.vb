﻿' automatically generated by the FlatBuffers compiler, do not modify
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

	Public NotInheritable Class OpType
	  Private Sub New()
	  End Sub
	  Public Const TRANSFORM_FLOAT As SByte = 0
	  Public Const TRANSFORM_SAME As SByte = 1
	  Public Const TRANSFORM_BOOL As SByte = 2
	  Public Const TRANSFORM_STRICT As SByte = 3
	  Public Const TRANSFORM_ANY As SByte = 4
	  Public Const REDUCE_FLOAT As SByte = 5
	  Public Const REDUCE_SAME As SByte = 6
	  Public Const REDUCE_LONG As SByte = 7
	  Public Const REDUCE_BOOL As SByte = 8
	  Public Const INDEX_REDUCE As SByte = 9
	  Public Const SCALAR As SByte = 10
	  Public Const SCALAR_BOOL As SByte = 11
	  Public Const BROADCAST As SByte = 12
	  Public Const BROADCAST_BOOL As SByte = 13
	  Public Const PAIRWISE As SByte = 14
	  Public Const PAIRWISE_BOOL As SByte = 15
	  Public Const REDUCE_3 As SByte = 16
	  Public Const SUMMARYSTATS As SByte = 17
	  Public Const SHAPE As SByte = 18
	  Public Const AGGREGATION As SByte = 19
	  Public Const RANDOM As SByte = 20
	  Public Const CUSTOM As SByte = 21
	  Public Const GRAPH As SByte = 22
	  Public Const VARIABLE As SByte = 40
	  Public Const [BOOLEAN] As SByte = 60
	  Public Const LOGIC As SByte = 119

	  Public Shared ReadOnly names() As String = { "TRANSFORM_FLOAT", "TRANSFORM_SAME", "TRANSFORM_BOOL", "TRANSFORM_STRICT", "TRANSFORM_ANY", "REDUCE_FLOAT", "REDUCE_SAME", "REDUCE_LONG", "REDUCE_BOOL", "INDEX_REDUCE", "SCALAR", "SCALAR_BOOL", "BROADCAST", "BROADCAST_BOOL", "PAIRWISE", "PAIRWISE_BOOL", "REDUCE_3", "SUMMARYSTATS", "SHAPE", "AGGREGATION", "RANDOM", "CUSTOM", "GRAPH", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "VARIABLE", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "BOOLEAN", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "LOGIC"}

	  Public Shared Function name(ByVal e As Integer) As String
		  Return names(e)
	  End Function
	End Class


End Namespace