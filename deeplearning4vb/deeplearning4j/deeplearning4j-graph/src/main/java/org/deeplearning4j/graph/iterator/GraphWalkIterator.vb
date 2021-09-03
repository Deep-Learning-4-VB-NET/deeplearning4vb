﻿Imports org.deeplearning4j.graph.api

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

Namespace org.deeplearning4j.graph.iterator

	Public Interface GraphWalkIterator(Of T)

		''' <summary>
		''' Length of the walks returned by next()
		''' Note that a walk of length {@code i} contains {@code i+1} vertices
		''' </summary>
		Function walkLength() As Integer

		''' <summary>
		'''Get the next vertex sequence.
		''' </summary>
		Function [next]() As IVertexSequence(Of T)

		''' <summary>
		''' Whether the iterator has any more vertex sequences. </summary>
		Function hasNext() As Boolean

		''' <summary>
		''' Reset the graph walk iterator. </summary>
		Sub reset()
	End Interface

End Namespace