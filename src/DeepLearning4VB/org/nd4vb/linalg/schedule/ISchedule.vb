Imports System

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

Namespace org.nd4vb.linalg.schedule

    ''' <summary>
    ''' A schedule for a hyperparameter (learning rate, momentum, ...) as a function of the iteration and epoch.
    ''' Java's <c>ISchedule</c> extends <c>Serializable</c> and the marker interface <c>Cloneable</c>; implementations
    ''' here are <c>&lt;Serializable&gt;</c> and <see cref="clone"/> is the typed copy (no <see cref="ICloneable"/>,
    ''' whose <c>Clone()</c> would clash with it). Java's Jackson annotation
    ''' (<c>@JsonTypeInfo(use = CLASS, property = "@class")</c>) is not applied yet: the JSON attribute layer lives in
    ''' <c>org.datavec.api.transform.serde</c>.
    ''' </summary>
    Public Interface ISchedule

        ''' <param name="iteration"> Current iteration number. Starts at 0 </param>
        ''' <param name="epoch">     Current epoch number. Starts at 0 </param>
        ''' <returns> Value at the current iteration/epoch for this schedule </returns>
        Function valueAt(ByVal iteration As Integer, ByVal epoch As Integer) As Double

        ''' <summary>A copy of this schedule.</summary>
        Function clone() As ISchedule

    End Interface

End Namespace
