Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement

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

Namespace org.deeplearning4j.models.sequencevectors.graph.walkers

	Public Interface GraphWalker(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: org.deeplearning4j.models.sequencevectors.graph.primitives.IGraph<T, ?> getSourceGraph();
		ReadOnly Property SourceGraph As IGraph(Of T, Object)

		''' <summary>
		''' This method checks, if walker has any more sequences left in queue
		''' 
		''' @return
		''' </summary>
		Function hasNext() As Boolean

		''' <summary>
		''' This method returns next walk sequence from this graph
		''' 
		''' @return
		''' </summary>
		Function [next]() As Sequence(Of T)

		''' <summary>
		''' This method resets walker
		''' </summary>
		''' <param name="shuffle"> if TRUE, order of walks will be shuffled </param>
		Sub reset(ByVal shuffle As Boolean)


		ReadOnly Property LabelEnabled As Boolean
	End Interface

End Namespace