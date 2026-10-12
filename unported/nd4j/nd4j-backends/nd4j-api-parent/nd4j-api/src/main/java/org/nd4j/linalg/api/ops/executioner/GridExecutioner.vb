Imports Aggregate = org.nd4j.linalg.api.ops.aggregates.Aggregate

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

Namespace org.nd4j.linalg.api.ops.executioner

	Public Interface GridExecutioner
		Inherits OpExecutioner

		''' <summary>
		''' This method forces all currently enqueued ops to be executed immediately
		''' 
		''' PLEASE NOTE: This call CAN be non-blocking, if specific backend implementation supports that.
		''' </summary>
		Sub flushQueue()

		''' <summary>
		''' This method forces all currently enqueued ops to be executed immediately
		''' 
		''' PLEASE NOTE: This call is always blocking, until all queued operations are finished
		''' </summary>
		Sub flushQueueBlocking()


		''' <summary>
		''' This method returns number of operations currently enqueued for execution
		''' 
		''' @return
		''' </summary>
		ReadOnly Property QueueLength As Integer


		''' <summary>
		''' This method enqueues aggregate op for future invocation
		''' </summary>
		''' <param name="op"> </param>
		Sub aggregate(ByVal op As Aggregate)

		''' <summary>
		''' This method enqueues aggregate op for future invocation.
		''' Key value will be used to batch individual ops
		''' </summary>
		''' <param name="op"> </param>
		''' <param name="key"> </param>
		Sub aggregate(ByVal op As Aggregate, ByVal key As Long)
	End Interface

End Namespace