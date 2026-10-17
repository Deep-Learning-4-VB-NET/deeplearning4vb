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

Imports DeepLearning4VB.org.nd4vb.common.function

Namespace org.datavec.api.transform.ops

    ''' <summary>
    ''' An aggregation that consumes elements one at a time (<see cref="Consumer(Of T)"/>), can be combined with
    ''' another accumulator of the same kind, and produces its result (<see cref="Supplier(Of T)"/>).
    ''' The Java generic method <c>&lt;W extends IAggregableReduceOp&lt;T, V&gt;&gt; void combine(W accu)</c> is the
    ''' plain <see cref="combine"/> with its bound as the parameter type. Implementations are serializable.
    ''' </summary>
    Public Interface IAggregableReduceOp(Of T, V)
        Inherits Consumer(Of T), Supplier(Of V)

        ''' <summary>Merges the state of another accumulator of the same kind into this one.</summary>
        Sub combine(ByVal accu As IAggregableReduceOp(Of T, V))

    End Interface

End Namespace
